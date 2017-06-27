using ElmRuntime2.Exceptions;
using ElmRuntime2.Expressions;
using ElmRuntime2.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser
{
    public static class LetParser
    {
        public static ParseResult<Let> ParseLet(TokenStream stream, int position, Module module)
        {
            if(!stream.IsAt(position, TokenType.Let))
            {
                throw new ParserException($"Unexpected start of let expression");
            }
            position++;

            var initialization = new List<Expression>();
            while (position <stream.Length && !stream.IsAt(position, TokenType.In))
            {
                //deconstructive pattern
                if (stream.IsAnyAt(position, TokenType.LeftParen, TokenType.LeftBrace))
                {
                    var pattern = PatternParser.ParsePattern(stream, position);
                    if (!pattern.Success)
                    {
                        throw new ParserException($"Invalid pattern in let expression");
                    }

                    initialization.Add(pattern.Value);
                    position = pattern.Position;
                }
                //type definition
                else if (stream.IsAt(position, TokenType.Identifier, TokenType.Colon))
                {
                    position = stream.SkipToNextExpression(position);
                }
                //function
                else if (stream.IsAt(position, TokenType.Identifier) && stream.ContainsInExpression(position, TokenType.Assign))
                {
                    var function = FunctionParser.ParseFunction(stream, position, module);
                    if (!function.Success)
                    {
                        throw new ParserException($"Invalid function in let expression");
                    }

                    initialization.Add(function.Value);
                    position = function.Position;
                }
                else
                {
                    throw new ParserException($"Unexpected expression in let");
                }
            }

            if (!stream.IsAt(position, TokenType.In))
            {
                throw new ParserException($"Unexpected token in let expression, expected in keyword");
            }
            position++;

            var resultParsed = ExpressionParser.ParseExpression(stream, position, module);
            if(!resultParsed.Success)
            {
                throw new ParserException($"Unexpected token in let expression, expected result expression");
            }

            var let = new Let(initialization.ToArray(), resultParsed.Value);
            return new ParseResult<Let>(true, let, resultParsed.Position);
        }
    }
}
