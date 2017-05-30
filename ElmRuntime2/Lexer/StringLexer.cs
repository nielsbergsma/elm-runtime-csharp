using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Lexer
{
    public class StringLexer : Lexer
    {
        private readonly Lexer source;
        private readonly Stack<Token> buffer;

        public StringLexer(Lexer source)
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
            var start = line.IndexOf("\"");
            if (start < 0)
            {
                return token;
            }

            var end = start + 1;
            for (var escaped = false; end < line.Length; end++)
            {
                if (line[end] == '\\')
                {
                    escaped = !escaped;
                }
                else if (line[end] == '\"' && !escaped)
                {
                    end++;
                    break;
                }
                else
                {
                    escaped = false;
                }
            }

            if (end < line.Length)
            {
                buffer.Push(new Token(token.Value.Line, token.Value.Column + end, TokenType.Unparsed, line.Substring(end)));
            }

            if (end - start > 2)
            {
                buffer.Push(new Token(token.Value.Line, token.Value.Column + start + 1, TokenType.String, line.Substring(start + 1, end - start - 2)));
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
