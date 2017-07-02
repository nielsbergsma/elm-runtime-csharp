using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Lexer
{
    public class KeywordLexer : Lexer
    {
        #region Keywords
        private readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
        {
            { "True", TokenType.True },
            { "False", TokenType.False },
            { "alias", TokenType.Alias },
            { "of", TokenType.Of },
            { "case", TokenType.Case },
            { "if", TokenType.If },
            { "then", TokenType.Then },
            { "else", TokenType.Else },
            { "type", TokenType.TypeDef },
            { "as", TokenType.As },
            { "let", TokenType.Let },
            { "in", TokenType.In },
            { "module", TokenType.Module },
            { "port", TokenType.Port },
            { "exposing", TokenType.Exposing },
            { "import", TokenType.Import },
            { "infix", TokenType.Infix },
            { "infixl", TokenType.Infixl },
            { "infixr", TokenType.Infixr },
        };
        #endregion

        private readonly Lexer source;

        public KeywordLexer(Lexer source)
        {
            this.source = source;
        }

        public Maybe<Token> Next()
        {
            var token = source.Next();
            if (!token.HasValue || !token.Value.Is(TokenType.Identifier))
            {
                return token;
            }            
            
            if (!keywords.ContainsKey(token.Value.Content))
            {
                return token;
            }

            var type = keywords[token.Value.Content];

            var offset = 0;
            if (type == TokenType.In)
            {
                offset = 1;
            }

            return Maybe<Token>.Some(new Token(token.Value.Line, token.Value.Column + offset, type, token.Value.Content));
        }

        public void Reset()
        {
            source.Reset();
        }
    }
}