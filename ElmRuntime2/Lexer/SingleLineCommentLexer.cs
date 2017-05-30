using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Lexer
{
    public class SingleLineCommentLexer : Lexer
    {
        private readonly Lexer source;
        private readonly Stack<Token> head;

        public SingleLineCommentLexer(Lexer source)
        {
            this.source = new SplitLexer("--", TokenType.SingleLineCommentStart, source);
            this.head = new Stack<Token>();
        }

        public Maybe<Token> Next()
        {
            if (head.Any())
            {
                return Maybe<Token>.Some(head.Pop());
            }

            var token = source.Next();
            if (!token.HasValue || !token.Value.Is(TokenType.SingleLineCommentStart))
            {
                return token;
            }

            var line = token.Value.Line;

            var content = new List<Token>();
            for (token = source.Next(); token.HasValue && token.Value.Line == line; token = source.Next())
            {
                content.Add(token.Value);
            }

            if (token.HasValue)
            {
                head.Push(token.Value);
            }

            var text = string.Join(" ", content.Select(c => c.Content));
            var comment = new Token(line, content[0].Column, TokenType.Comment, text);
            return Maybe<Token>.Some(comment);
        }

        public void Reset()
        {
            head.Clear();
            source.Reset();
        }
    }
}
