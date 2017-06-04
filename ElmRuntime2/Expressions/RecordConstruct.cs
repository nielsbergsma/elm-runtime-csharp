using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElmRuntime2.Lexer;
using ElmRuntime2.Values;
using ElmRuntime2.Parser;

namespace ElmRuntime2.Expressions
{
    public class RecordConstruct : Expression
    {
        private readonly Dictionary<string, Expression> fieldExpressions;

        public RecordConstruct(Dictionary<string, Expression> fieldExpressions)
        {
            this.fieldExpressions = fieldExpressions;
        } 

        public Expression Evaluate(Value[] arguments, Scope scope)
        {
            var record = new Record();

            var fieldValues = new List<RecordFieldValue>();
            foreach (var fieldExpression in fieldExpressions)
            {
                var value = fieldExpression.Value.Evaluate(arguments, scope) as Value;
                var field = new RecordFieldValue(fieldExpression.Key, value);
                fieldValues.Add(field);
            }

            return record.SetFields(fieldValues.ToArray());
        }

        public static ParseResult<RecordConstruct> Parse(TokenStream stream, int position)
        {
            var parsed = ParserHelper.ParseList(stream, position);

            var fieldExpressions = new Dictionary<string, Expression>();
            foreach (var assignment in parsed.Value)
            {
                var fieldExpression = ExpressionParser.ParseExpression(assignment, 2);
                if (assignment.IsAt(0, TokenType.Identifier, TokenType.Assign) && fieldExpression.Success)
                {
                    var fieldName = assignment.At(0).Content;
                    fieldExpressions[fieldName] = fieldExpression.Value;
                }
            }

            var construction = new RecordConstruct(fieldExpressions);
            return new ParseResult<RecordConstruct>(true, construction, parsed.Position);
        }
    }
}
