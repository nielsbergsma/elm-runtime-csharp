using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Lexer
{
    public class SplitLexer : Lexer
    {
        private readonly string seperator;
        private readonly TokenType tokenType;

        private readonly Lexer source;
        private readonly Stack<Token> buffer;

        public SplitLexer(string seperator, TokenType tokenType, Lexer source)
        {
            this.seperator = seperator;
            this.tokenType = tokenType;
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

            var index = token.Value.Content.IndexOf(seperator);
            if (index < 0)
            {
                return token;
            }

            var split = token.Value.Split(index, seperator);

            if (split.After.HasValue)
            {
                buffer.Push(split.After.Value);
            }

            buffer.Push(new Token(token.Value.Line, index, tokenType, seperator));

            if (split.Before.HasValue)
            {
                buffer.Push(split.Before.Value);
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
