using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Lexer
{
    public class StringLexer : Lexer
    {
        private const char quote = '\"';
        private const char blackSlash = '\\';

        private readonly Lexer source;
        private readonly Stack<Token> head;

        public StringLexer(Lexer source)
        {
            this.source = source;
            this.head = new Stack<Token>();
        }

        public Maybe<Token> Next()
        {
            var token = head.Any()
                ? Maybe<Token>.Some(head.Pop())
                : source.Next();

            if (!token.HasValue || !token.Value.Is(TokenType.Unparsed))
            {
                return token;
            }

            var line = token.Value.Content;
            var start = line.IndexOf(quote);
            if (start < 0)
            {
                return token;
            }

            var end = start + 1;
            var ended = false;
            for (var escaped = false; !ended && end < line.Length; end++)
            {
                if (line[end] == blackSlash)
                {
                    escaped = !escaped;
                }
                else if (line[end] == quote && !escaped)
                {
                    end++;
                    ended = true;
                }
                else
                {
                    escaped = false;
                }
            }

            if (!ended)
            {
                end = line.Length;
            }

            if (end < line.Length)
            {
                head.Push(new Token(token.Value.Line, token.Value.Column + end, TokenType.Unparsed, line.Substring(end)));
            }

            if (!ended)
            {
                head.Push(new Token(token.Value.Line, token.Value.Column + start, TokenType.Unknown, line.Substring(start)));
            }
            else
            {
                head.Push(new Token(token.Value.Line, token.Value.Column + start + 1, TokenType.String, line.Substring(start + 1, end - start - 2)));
            }

            if (start > 0)
            {
                head.Push(new Token(token.Value.Line, token.Value.Column, TokenType.Unparsed, line.Substring(0, start)));
            }

            return Next();
        }

        public void Reset()
        {
            head.Clear();
            source.Reset();
        }
    }
}
