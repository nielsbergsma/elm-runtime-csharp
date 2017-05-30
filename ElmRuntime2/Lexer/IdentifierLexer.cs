using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ElmRuntime2.Lexer
{
    public class IdentifierLexer : Lexer
    {
        private readonly Lexer source;
        private readonly Stack<Token> buffer;
        private readonly static Regex identifier = new Regex(@"[a-z][a-z0-9_]*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public IdentifierLexer(Lexer source)
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

            var content = token.Value.Content;
            var match = identifier.Match(content);
            if (!match.Success || IsSurroundedByNumber(content, match.Index, match.Index + match.Length))
            {
                return token;
            }

            var start = match.Index;
            var end = start + match.Length;
            if (end < content.Length)
            {
                buffer.Push(new Token(token.Value.Line, token.Value.Column + end, TokenType.Unparsed, content.Substring(end)));
            }

            buffer.Push(new Token(token.Value.Line, token.Value.Column + start, TokenType.Identifier, match.Value));

            if (start > 0)
            {
                buffer.Push(new Token(token.Value.Line, token.Value.Column, TokenType.Unparsed, content.Substring(0, start)));
            }

            return Next();
        }

        private bool IsSurroundedByNumber(string content, int start, int end)
        {
            if (start > 0 && char.IsDigit(content[start - 1]))
            {
                return true;
            }
            else if (end < content.Length && char.IsDigit(content[end]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reset()
        {
            buffer.Clear();
            source.Reset();
        }
    }
}
