using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Lexer
{
    public class MultiLineStringLexer : Lexer
    {
        private const string boundry = @"""""""";
        private const string newLine = "\n";

        private readonly Lexer source;

        public MultiLineStringLexer(Lexer source)
        {
            this.source = new SplitLexer(boundry, TokenType.MultiLineStringBoundry, source);
        }

        public Maybe<Token> Next()
        {
            while (true)
            {
                var token = source.Next();
                if (!token.HasValue || !token.Value.Is(TokenType.MultiLineStringBoundry))
                {
                    return token;
                }

                var content = new List<Token>();
                for (token = source.Next(); token.HasValue; token = source.Next())
                {
                    if (token.Value.Is(TokenType.MultiLineStringBoundry))
                    {
                        break;
                    }
                    else
                    {
                        content.Add(token.Value);
                    }
                }

                if (content.Any())
                {
                    var start = content[0];
                    var text = string.Empty;

                    for (int c = 0, l = start.Line; c < content.Count; l = content[c].Line, c++)
                    {
                        text += content[c].Line == l ? string.Empty : newLine;
                        text += content[c].Content;
                    }

                    return Maybe<Token>.Some(new Token(start.Line, start.Column, TokenType.String, text));
                }
            }
        }

        public void Reset()
        {
            source.Reset();
        }
    }
}
