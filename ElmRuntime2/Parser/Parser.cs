using ElmRuntime2.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser
{
    public static class Parser
    {
        public static ParseResult<TokenStream[]> ParseList(TokenStream stream, int position)
        {
            var list = new List<TokenStream>();
            var seperator = TokenType.Comma;
            var start = position;
            var stop = TokenType.RightParen;

            if (start >= stream.Length)
            {
                return new ParseResult<TokenStream[]>(false, new TokenStream[0], position);
            }

            switch(stream.At(start).Type)
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
                    if (stream.IsAt(position, TokenType.LeftParen) || stream.IsAt(position, TokenType.LeftBrace) || stream.IsAt(position, TokenType.LeftBracket))
                    {
                        nesting++;
                    }
                    else if (stream.IsAt(position, TokenType.RightParen) || stream.IsAt(position, TokenType.RightBrace) || stream.IsAt(position, TokenType.RightBracket))
                    {
                        nesting--;
                    }
                }
            }

            return new ParseResult<TokenStream[]>(list.ToArray(), position);
        }
    }
}
