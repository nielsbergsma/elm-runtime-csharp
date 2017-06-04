using ElmRuntime2.Exceptions;
using ElmRuntime2.Lexer;
using ElmRuntime2.Parser;
using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public class RecordUpdate : Expression
    {
        private readonly string name;
        private readonly Dictionary<string, Expression> fieldExpressions;

        public RecordUpdate(string name, Dictionary<string, Expression> fieldExpressions)
        {
            this.name = name;
            this.fieldExpressions = fieldExpressions;
        }

        public Expression Evaluate(Value[] arguments, Scope scope)
        {
            var recordValue = default(Value);
            if (!scope.TryGetValue(name, out recordValue) || !(recordValue is Record))
            {
                throw new RuntimeException($"Variable {name} is not a record");
            }
            var record = recordValue as Record;

            var fieldValues = new List<RecordFieldValue>();
            foreach (var fieldExpression in fieldExpressions)
            {
                var value = fieldExpression.Value.Evaluate(arguments, scope) as Value;
                var field = new RecordFieldValue(fieldExpression.Key, value);
                fieldValues.Add(field);
            }

            return record.SetFields(fieldValues.ToArray());
        }

        public static ParseResult<RecordUpdate> Parse(TokenStream stream, int position)
        {
            if (!stream.IsAt(position, TokenType.LeftBrace, TokenType.Identifier, TokenType.Pipe))
            {
                throw new ParserException($"Unable to parse record update near line { stream.LineOf(position) }");
            }

            var parsed = ParserHelper.ParseList(stream, position);

            var name = stream.At(position + 1).Content;
            var fieldExpressions = new Dictionary<string, Expression>();

            if (parsed.Value.Length > 0 && parsed.Value[0].IsAt(2, TokenType.Identifier, TokenType.Assign))
            {
                var assignment = parsed.Value[0];
                var fieldName = assignment.At(2).Content;
                var fieldExpression = ExpressionParser.ParseExpression(assignment, 4);

                fieldExpressions[fieldName] = fieldExpression.Value;
            }

            for (var fe = 1; fe < parsed.Value.Length; fe++)
            {
                var assignment = parsed.Value[fe];
                var fieldExpression = ExpressionParser.ParseExpression(assignment, 2);
                if (assignment.IsAt(0, TokenType.Identifier, TokenType.Assign) && fieldExpression.Success)
                {
                    var fieldName = assignment.At(0).Content;
                    fieldExpressions[fieldName] = fieldExpression.Value;
                }
            }

            var update = new RecordUpdate(name, fieldExpressions);
            return new ParseResult<RecordUpdate>(true, update, parsed.Position);
        }
    }
}
