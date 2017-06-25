using ElmRuntime2.Exceptions;
using ElmRuntime2.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser
{
    public static class ListParser
    {
        public static ParseResult<ListConstruct> ParseList(TokenStream stream, int position, Module module)
        {
            var parsed = ParserHelper.ParseArray(stream, position);
            if (!parsed.Success)
            {
                return new ParseResult<ListConstruct>(false, default(ListConstruct), parsed.Position);
            }

            var expressions = new List<Expression>();
            foreach (var tokens in parsed.Value)
            {
                var expression = ExpressionParser.ParseExpression(tokens, 0, module);
                if (!expression.Success)
                {
                    throw new ParserException($"Unable to parse list item expression near line {stream.LineOf(position)}");
                }
                expressions.Add(expression.Value);
            }

            var construction = new ListConstruct(expressions.ToArray());
            return new ParseResult<ListConstruct>(true, construction, parsed.Position);
        }
    }
}
