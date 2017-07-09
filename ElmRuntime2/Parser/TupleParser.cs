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
    public static class TupleParser
    {
        public static ParseResult<TupleConstruct> ParseTuple(TokenStream stream, int position, Module module)
        {
            if (stream.IsAt(position, TokenType.LeftParen, TokenType.Comma))
            {
                var numberOfElements = 0;
                for (; position < stream.Length && !stream.IsAt(position, TokenType.RightParen); position++)
                {
                    numberOfElements++;
                }

                if (stream.IsAt(position, TokenType.RightParen))
                {
                    position++;
                }

                var expressions = new List<Expression>();
                for (var e = 0; e < numberOfElements; e++)
                {
                    var expression = ExpressionParser.ParseExpression(stream, position, module, false);
                    if (expression.Success)
                    {
                        expressions.Add(expression.Value);
                    }
                    else
                    {
                        throw new ParserException($"Unable to parse expression in tuple near line ${stream.LineOf(position)}");
                    }
                    position = expression.Position;
                }

                var tupleConstruct = new TupleConstruct(expressions.ToArray());
                return new ParseResult<TupleConstruct>(true, tupleConstruct, position);
            }

            var array = ParserHelper.ParseArray(stream, position);
            if (array.Value.Length > 1)
            {
                var expressions = new List<Expression>();
                foreach (var element in array.Value)
                {
                    var expression = ExpressionParser.ParseExpression(element, 0, module, true);
                    if (expression.Success)
                    {
                        expressions.Add(expression.Value);
                    }
                    else
                    {
                        throw new ParserException($"Unable to parse expression in tuple near line ${stream.LineOf(position)}");
                    }
                }

                var tupleConstruct = new TupleConstruct(expressions.ToArray());
                return new ParseResult<TupleConstruct>(true, tupleConstruct, array.Position);
            }

            throw new ParserException($"Unable to parse tuple expression near line {stream.LineOf(position)}");
        }
    }
}
