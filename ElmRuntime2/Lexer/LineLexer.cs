using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Lexer
{
    public class LineLexer : Lexer
    {
        private readonly List<Token> tokens;
        private int position;
        private bool reset;

        public LineLexer(string input)
        {
            tokens = new List<Token>();
            reset = true;

            var lines = input.Split('\n');
            for (var l = lines.Length - 1; l >= 0; l--)
            {
                var token = new Token(l, 0, TokenType.Unparsed, lines[l]);
                tokens.Insert(0, token);
            }
        }

        public Maybe<Token> Next()
        {
            if (reset)
            {
                if (tokens.Any())
                {
                    reset = false;
                    position = 0;
                    return Maybe<Token>.Some(tokens[position]);
                }
            }
            else if (position + 1 < tokens.Count)
            {
                position++;
                return Maybe<Token>.Some(tokens[position]);
            }

            return Maybe<Token>.None();
        }

        public void Reset()
        {
            reset = true;
        }
    }
}
