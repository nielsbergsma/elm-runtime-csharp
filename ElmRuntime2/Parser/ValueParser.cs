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
    public static class ValueParser
    {
        public static ParseResult<Values.Value> ParseValue(TokenStream stream, int position)
        {
            if (position >= stream.Length)
            {
                return new ParseResult<Values.Value>(false, default(Values.Value), position);
            }

            if (stream.IsAt(position, TokenType.Int))
            {
                var value = int.Parse(stream.At(position).Content);
                return new ParseResult<Values.Value>(true, new Values.Integer(value), position + 1);
            }
            else if (stream.IsAt(position, TokenType.Float))
            {
                var value = float.Parse(stream.At(position).Content);
                return new ParseResult<Values.Value>(true, new Values.Float(value), position + 1);
            }
            else if (stream.IsAt(position, TokenType.Char))
            {
                var value = stream.At(position).Content[0];
                return new ParseResult<Values.Value>(true, new Values.Character(value), position + 1);
            }
            else if (stream.IsAt(position, TokenType.String))
            {
                var value = stream.At(position).Content;
                return new ParseResult<Values.Value>(true, new Values.String(value),  position + 1);
            }
            else if (stream.IsAt(position, TokenType.True))
            {
                return new ParseResult<Values.Value>(true, new Values.Boolean(true), position + 1);
            }
            else if (stream.IsAt(position, TokenType.False))
            {
                return new ParseResult<Values.Value>(true, new Values.Boolean(false),  position + 1);
            }
            else
            {
                throw new ParserException($"Cannot value from token {stream.At(position).Type}");
            }
        }
    }
}
