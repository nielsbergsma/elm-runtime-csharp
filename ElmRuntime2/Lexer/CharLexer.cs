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
        private readonly Stack<Token> head;

        public CharLexer(Lexer source)
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
            var start = line.IndexOf("\'");
            if (start < 0)
            {
                return token;
            }

            var end = start + 1;
            if (start + 2 < line.Length && line[start + 2] == '\'')
            {
                end = start + 3;
            }
            else if (start + 3 < line.Length && line[start + 1] == '\\' &&  line[start + 3] == '\'')
            {
                end = start + 4;
            }

            if (end + 1 < line.Length)
            {
                head.Push(new Token(token.Value.Line, token.Value.Column + end, TokenType.Unparsed, line.Substring(end)));
            }

            if (end - start == 1)
            {
                head.Push(new Token(token.Value.Line, token.Value.Column + start, TokenType.Unknown, line.Substring(start, 1)));
            }
            else
            {
                head.Push(new Token(token.Value.Line, token.Value.Column + start + 1, TokenType.Char, line.Substring(start + 1, end - start - 2)));
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
