using ElmRuntime2.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser
{
    public static class OperatorParser
    {
        public static TokenStream Resolve(TokenStream stream, int position, Module module)
        {
            var end = stream.SkipToNextExpression(position);

            var tokens = new List<Token>();
            for (; position < end; position++)
            {
                tokens.Add(stream.At(position));
            }

            var operators = module.Operators.OrderByDescending(o => o.Symbol.Length).ToArray();
            foreach(var @operator in operators)
            {
                var result = new List<Token>();
                var match = @operator.Symbol;
                var size = @operator.Symbol.Length;

                for (var t = 0; t < tokens.Count; t++)
                {
                    var matched = t < (tokens.Count - match.Length);
                    for (var m = 0; matched && m < match.Length; m++)
                    {
                        matched = tokens[t + m].Content == match[m].ToString();
                    }

                    if (matched)
                    {
                        var prefix = t > 0 && tokens[t - 1].Type == TokenType.LeftParen
                            && t + size < tokens.Count && tokens[t + size].Type == TokenType.RightParen;
                        
                        if (prefix)
                        {
                            var token = new Token(tokens[t].Line, tokens[t].Column, TokenType.OpPrefix, @operator.Symbol);
                            result.RemoveAt(result.Count - 1);
                            result.Add(token);
                            t += size;
                        }
                        else
                        {
                            var type = tokens[t].Type == TokenType.OpPrefix ? TokenType.OpPrefix : TokenType.OpInfix;
                            var token = new Token(tokens[t].Line, tokens[t].Column, type, @operator.Symbol);
                            result.Add(token);
                            t += size - 1;
                        }
                    }
                    else
                    {
                        result.Add(tokens[t]);
                    }
                }

                tokens = result;
            }

            return new TokenStream(tokens.ToArray());
        }
    }
}
