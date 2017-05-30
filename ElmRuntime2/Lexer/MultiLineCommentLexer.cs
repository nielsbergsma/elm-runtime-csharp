using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Lexer
{
    public class MultiLineCommentLexer : Lexer
    {
        private readonly Lexer source;
        private readonly Stack<Token> buffer;
 
        public MultiLineCommentLexer(Lexer source)
        {
            this.source = source;
            this.source = new SplitLexer("{-", TokenType.MultiLineCommentStart, this.source);
            this.source = new SplitLexer("-}", TokenType.MultiLineCommentEnd, this.source);
            this.buffer = new Stack<Token>();
        }

        public Maybe<Token> Next()
        {
            var token = buffer.Any()
                ? Maybe<Token>.Some(buffer.Pop())
                : source.Next();

            if (!token.HasValue || !token.Value.Is(TokenType.MultiLineCommentStart))
            {
                return token;
            }
            token = source.Next();

            var content = new List<Token>();
            for (var nesting = 1; nesting > 0 && token.HasValue; token = source.Next())
            {
                if (token.Value.Is(TokenType.MultiLineCommentStart))
                {
                    nesting++;
                }
                else if (token.Value.Is(TokenType.MultiLineCommentEnd))
                {
                    nesting--;
                }
                else
                {
                    content.Add(token.Value);
                }
            }

            if (token.HasValue)
            {
                buffer.Push(token.Value);
            }

            if (content.Any())
            {
                var start = content[0];
                var text = string.Empty;

                for (int c = 0, l = start.Line; c < content.Count; l = content[c].Line, c++)
                {
                    text += content[c].Line == l ? "" : "\n";
                    text += content[c].Content;
                }

                return Maybe<Token>.Some(new Token(start.Line, start.Column, TokenType.Comment, text));
            }
            else
            {
                return Next();
            }
        }

        public void Reset()
        {
            buffer.Clear();
            source.Reset();
        }
    }
}
