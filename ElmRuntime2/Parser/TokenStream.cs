using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElmRuntime2.Lexer;

namespace ElmRuntime2.Parser
{
    public class TokenStream
    {
        private readonly Token[] tokens;

        public TokenStream(Lexer.Lexer lexer)
        {
            var tokens = new List<Token>();
            for (var token = lexer.Next(); token.HasValue; token = lexer.Next())
            {
                tokens.Add(token.Value);
            }
            this.tokens = tokens.ToArray();
        }

        public TokenStream(Token[] tokens)
        {
            this.tokens = tokens;
        }

        public int Length
        {
            get { return tokens.Length; }
        }

        public bool IsAtStartOfLine(int position)
        {
            return (tokens.Length > 0 && position == 0) || (position < tokens.Length && tokens[position].Line != tokens[position - 1].Line);
        }

        public bool IsAtEndOfStream(int position)
        {
            return position + 1 >= tokens.Length;
        }

        public bool IsAt(int position, TokenType type)
        {
            return position < tokens.Length && tokens[position].Type == type;
        }

        public bool IsAt(int position, TokenType type, params TokenType[] rest)
        {
            if (position >= tokens.Length || tokens[position].Type != type)
            {
                return false;
            }

            for (var r = 0; r < rest.Length; r++)
            {
                if (position + 1 + r>= tokens.Length || tokens[position + 1 + r].Type != rest[r])
                {
                    return false;
                }
            }

            return true;
        }

        public Token At(int position)
        {
            return tokens[position];
        }

        public Maybe<int> FindOnLine(int position, TokenType type)
        {
            if (position >= tokens.Length)
            {
                return Maybe<int>.None();
            }

            var line = tokens[position].Line;
            for (; position < tokens.Length && tokens[position].Line == line; position++)
            {
                if (tokens[position].Type == type)
                {
                    return Maybe<int>.Some(position);
                }
            }

            return Maybe<int>.None();
        }

        public int SkipUntil(int position, TokenType type)
        {
            if (position >= tokens.Length)
            {
                return tokens.Length - 1;
            }

            for (; position < tokens.Length && tokens[position].Type != type; position++)
            {
                //forward position
            }

            if (tokens[position].Type == type)
            {
                position++;
            }

            position++;
            return position;
        }

        public int SkipUntilNextLine(int position)
        {
            if (position >= tokens.Length)
            {
                return tokens.Length - 1;
            }

            var line = tokens[position].Line;
            for (; position < tokens.Length && tokens[position].Line == line; position++)
            {
                //forward position
            }

            return position;
        }
    }
}
