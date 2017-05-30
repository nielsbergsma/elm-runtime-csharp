using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Lexer
{
    public class SymbolLexer : Lexer
    {
        #region Symbols
        private readonly static Dictionary<string, TokenType>[] symbols = new[] {
            new Dictionary<string, TokenType>
            {
                { ">>", TokenType.Op },
                { "<<", TokenType.Op },
                { "//", TokenType.Op },
                { "++", TokenType.Op },
                { "::", TokenType.Op },
                { "==", TokenType.Op },
                { "/=", TokenType.Op },
                { "<=", TokenType.Op },
                { ">=", TokenType.Op },
                { "&&", TokenType.Op },
                { "||", TokenType.Op },
                { "<|", TokenType.Op },
                { "|>", TokenType.Op },
                { "->", TokenType.Arrow },
                { "..", TokenType.Range }
            },

            new Dictionary<string, TokenType>
            {
                { "<", TokenType.Op },
                { ">", TokenType.Op },
                { "/", TokenType.Op },
                { "^", TokenType.Op },
                { "*", TokenType.Op },
                { "+", TokenType.Op },
                { "-", TokenType.Op },
                { "%", TokenType.Op },
                { ".", TokenType.Dot },
                { "\\", TokenType.Backslash },
                { "|", TokenType.Pipe },
                { ",", TokenType.Comma },
                { ":", TokenType.Colon },
                { "=", TokenType.Assign },
                { "(", TokenType.LeftParen },
                { ")", TokenType.RightParen },
                { "{", TokenType.LeftBrace },
                { "}", TokenType.RightBrace },
                { "[", TokenType.LeftBracket },
                { "]", TokenType.RightBracket },
            }
        };
        #endregion

        private readonly Lexer source;
        private readonly Stack<Token> head;

        public SymbolLexer(Lexer source)
        {
            this.source = source;
            this.head = new Stack<Token>();
        }

        public Maybe<Token> Next()
        {
            var token = head.Any()
                ? Maybe<Token>.Some(head.Pop())
                : source.Next();

            if (!token.HasValue || !token.Value.Is(TokenType.Unparsed))
            {
                return token;
            }

            var content = token.Value.Content;
            foreach (var group in symbols)
            {
                var length = (group.First().Key.Length);

                for (var start = 0; start + length <= content.Length; start++)
                {
                    var slice = content.Substring(start, length);
                    if (!group.ContainsKey(slice))
                    {
                        continue;
                    }

                    var end = start + length;
                    if (end < content.Length)
                    {
                        head.Push(new Token(token.Value.Line, token.Value.Column + end, TokenType.Unparsed, content.Substring(end)));
                    }

                    var type = group[slice];
                    head.Push(new Token(token.Value.Line, token.Value.Column + start, type, slice));

                    if (start > 0)
                    {
                        head.Push(new Token(token.Value.Line, token.Value.Column, TokenType.Unparsed, content.Substring(0, start)));
                    }

                    return Next();
                }
            }

            return token;
        }

        public void Reset()
        {
            head.Clear();
            source.Reset();
        }
    }
}