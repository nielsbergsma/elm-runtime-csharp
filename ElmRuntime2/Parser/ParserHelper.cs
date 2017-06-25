using ElmRuntime2.Exceptions;
using ElmRuntime2.Expressions;
using ElmRuntime2.Lexer;
using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser
{
    public static class ParserHelper
    {
        public static ParseResult<TokenStream[]> ParseArray(TokenStream stream, int position)
        {
            var list = new List<TokenStream>();
            var seperator = TokenType.Comma;
            var stop = TokenType.RightParen;

            if (position >= stream.Length)
            {
                return new ParseResult<TokenStream[]>(false, new TokenStream[0], position);
            }

            switch(stream.At(position).Type)
            {
                case TokenType.LeftParen:
                    stop = TokenType.RightParen;
                    break;

                case TokenType.LeftBrace:
                    stop = TokenType.RightBrace;
                    break;

                case TokenType.LeftBracket:
                    stop = TokenType.RightBracket;
                    break;

                default:
                    return new ParseResult<TokenStream[]>(false, new TokenStream[0], position);
            }

            var nesting = 0;
            var stack = new List<Token>();

            for (position++; position < stream.Length; position++)
            {
                if (stream.IsAt(position, seperator) && nesting == 0)
                {
                    if (stack.Any())
                    {
                        list.Add(new TokenStream(stack.ToArray()));
                        stack.Clear();
                    }
                }
                else if (stream.IsAt(position, stop) && nesting == 0)
                {
                    if (stack.Any())
                    {
                        list.Add(new TokenStream(stack.ToArray()));
                        stack.Clear();
                    }

                    position++;
                    break;
                }
                else
                {
                    stack.Add(stream.At(position));
                    if (stream.IsAnyAt(position, TokenType.LeftParen, TokenType.LeftBrace, TokenType.LeftBracket))
                    {
                        nesting++;
                    }
                    else if (stream.IsAnyAt(position, TokenType.RightParen, TokenType.RightBrace, TokenType.RightBracket))
                    {
                        nesting--;
                    }
                }
            }

            return new ParseResult<TokenStream[]>(list.ToArray(), position);
        }       

        public static bool IsVariableName(TokenStream stream, int position)
        {
            if (stream.IsAt(position, TokenType.Identifier))
            {
                var content = stream.At(position).Content;
                return content.Length > 0 && char.IsLower(content[0]);
            }

            return false;
        }

        public static bool IsCtorName(TokenStream stream, int position)
        {
            if (stream.IsAt(position, TokenType.Identifier))
            {
                var content = stream.At(position).Content;
                return content.Length > 0 && char.IsUpper(content[0]);
            }

            return false;
        }
    }
}
