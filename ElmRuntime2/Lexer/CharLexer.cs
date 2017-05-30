using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Lexer
{
    public class CharLexer : Lexer
    {
        private readonly Lexer source;
        private readonly Stack<Token> buffer;

        public CharLexer(Lexer source)
        {
            this.source = source;
            this.buffer = new Stack<Token>();
        }

        public Maybe<Token> Next()
        {
            var token = buffer.Any()
                ? Maybe<Token>.Some(buffer.Pop())
                : source.Next();

            if (!token.HasValue || !token.Value.Is(TokenType.Unparsed))
            {
                return token;
            }

            var line = token.Value.Content;
            var start = line.IndexOf("\'");
            if (start < 0)
            {
                return token;
            }

            var end = start;
            if (start + 3 < line.Length && line[start + 2] == '\'')
            {
                end = start + 3;
            }
            else if (start + 4 < line.Length && line[start + 1] == '\\' &&  line[start + 3] == '\'')
            {
                end = start + 4;
            }

            if (start < end)
            {
                if (end + 1 < line.Length)
                {
                    buffer.Push(new Token(token.Value.Line, token.Value.Column + end, TokenType.Unparsed, line.Substring(end)));
                }

                buffer.Push(new Token(token.Value.Line, token.Value.Column + start + 1, TokenType.Char, line.Substring(start + 1, end - start - 2)));
            }
            else
            {
                //missing end quote
                buffer.Push(new Token(token.Value.Line, token.Value.Column + start, TokenType.Unknown, line.Substring(start)));
            }

            if (start > 0)
            {
                buffer.Push(new Token(token.Value.Line, token.Value.Column, TokenType.Unparsed, line.Substring(0, start)));
            }

            return Next();
        }

        public void Reset()
        {
            buffer.Clear();
            source.Reset();
        }

    }
}
