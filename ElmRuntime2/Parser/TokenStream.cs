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

        public bool AtStartOfExpression(int position)
        {
            if (position >= tokens.Length)
            {
                return false;
            }

            return tokens[position].Column == 0;
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

        public bool IsAt(int position, string content)
        {
            return position < tokens.Length && tokens[position].Content == content;
        }

        public bool IsAnyAt(int position, params TokenType[] types)
        {
            if (position >= tokens.Length)
            {
                return false;
            }
            return types.Contains(tokens[position].Type);
        }

        public Token At(int position)
        {
            return tokens[position];
        }

        public Maybe<int> FindInExpression(int position, TokenType type)
        {
            if (position >= tokens.Length)
            {
                return Maybe<int>.None();
            }

            var line = tokens[position].Line;
            while (position < tokens.Length && (tokens[position].Line == line || tokens[position].Column != 0))
            {
                if (tokens[position].Type == type)
                {
                    return Maybe<int>.Some(position);
                }
                position++;
            }

            return Maybe<int>.None();
        }

        public bool ContainsInExpression(int position, TokenType type)
        {
            return FindInExpression(position, type).HasValue;
        }

        public int LineOf(int position)
        {
            if (position >= tokens.Length)
            {
                return -1;
            }
            else
            {
                return tokens[position].Line;
            }
        }

        public int IdententationOfExpressionStart(int position)
        {
            if (position >= tokens.Length)
            {
                return 0;
            }

            var line = tokens[position].Line;
            var identation = tokens[position].Column;

            for (; position >= 0 && tokens[position].Line == line; position--)
            {
                identation = tokens[position].Column;
            }

            return identation;
        }

        public int SkipToNextExpression(int position)
        {
            if (position >= tokens.Length)
            {
                return tokens.Length - 1;
            }

            var identation = IdententationOfExpressionStart(position);

            position++;
            while (position < tokens.Length && tokens[position].Column > identation)
            {
                position++;
            }

            return position;
        }
    }
}
