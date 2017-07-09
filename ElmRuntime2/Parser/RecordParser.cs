using ElmRuntime2.Exceptions;
using ElmRuntime2.Expressions;
using ElmRuntime2.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser
{
    public static class RecordParser
    {
        public static ParseResult<RecordConstruct> ParseRecordConstruct(TokenStream stream, int position, Module module)
        {
            var parsed = ParserHelper.ParseArray(stream, position);

            var fieldExpressions = new Dictionary<string, Expression>();
            foreach (var assignment in parsed.Value)
            {
                var fieldExpression = ExpressionParser.ParseExpression(assignment, 2, module, true);
                if (assignment.IsAt(0, TokenType.Identifier, TokenType.Assign) && fieldExpression.Success)
                {
                    var fieldName = assignment.At(0).Content;
                    fieldExpressions[fieldName] = fieldExpression.Value;
                }
            }

            var construction = new RecordConstruct(fieldExpressions);
            return new ParseResult<RecordConstruct>(true, construction, parsed.Position);
        }

        public static ParseResult<RecordUpdate> ParseRecordUpdate(TokenStream stream, int position, Module module)
        {
            if (!stream.IsAt(position, TokenType.LeftBrace, TokenType.Identifier, TokenType.Pipe))
            {
                throw new ParserException($"Unable to parse record update near line { stream.LineOf(position) }");
            }

            var parsed = ParserHelper.ParseArray(stream, position);

            var name = stream.At(position + 1).Content;
            var fieldExpressions = new Dictionary<string, Expression>();

            if (parsed.Value.Length > 0 && parsed.Value[0].IsAt(2, TokenType.Identifier, TokenType.Assign))
            {
                var assignment = parsed.Value[0];
                var fieldName = assignment.At(2).Content;
                var fieldExpression = ExpressionParser.ParseExpression(assignment, 4, module, true);

                fieldExpressions[fieldName] = fieldExpression.Value;
            }

            for (var fe = 1; fe < parsed.Value.Length; fe++)
            {
                var assignment = parsed.Value[fe];
                var fieldExpression = ExpressionParser.ParseExpression(assignment, 2, module, true);
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
