using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Lexer
{
    public class UnparsedToUndeterminedLexer : Lexer
    {
        private readonly Lexer source;
        private readonly Stack<Token> head;

        public UnparsedToUndeterminedLexer(Lexer source)
        {
            this.source = source;
            this.head = new Stack<Token>();
        }

        public Maybe<Token> Next()
        {
            while (true)
            {
                if (head.Count > 0)
                {
                    return Maybe<Token>.Some(head.Pop());
                }

                var token = source.Next();
                if (!token.HasValue || token.Value.Type != TokenType.Unparsed)
                {
                    return token;
                }

                var sanitisedContent = token.Value.Content.Trim();
                var columnOffset = token.Value.Content.IndexOf(sanitisedContent);
                for(var sc = sanitisedContent.Length - 1; sc >= 0; sc--)
                {
                    if (!char.IsWhiteSpace(sanitisedContent[sc]))
                    {
                        head.Push(new Token(token.Value.Line, token.Value.Column + columnOffset + sc, TokenType.Undetermined, sanitisedContent[sc].ToString()));
                    }
                }
            }
        }

        public void Reset()
        {
            source.Reset();
        }
    }
}
