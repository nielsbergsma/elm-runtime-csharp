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
    public static class FunctionParser
    {
        public static ParseResult<Function> ParseFunction(TokenStream stream, int position, Module module)
        {
            var name = "";

            //regular identifier
            if (stream.IsAt(position, TokenType.Identifier))
            {
                name = stream.At(position).Content;
                position++;
            }
            //operator
            else if (stream.IsAt(position, TokenType.LeftParen) && stream.ContainsInExpression(position + 1, TokenType.RightParen))
            {
                var end = stream.FindInExpression(position + 1, TokenType.RightParen);
                for (position++; position < end.Value; position++)
                {
                    name += stream.At(position).Content;
                }
            }
            else
            {
                throw new ParserException($"Expected idenfier or operator start function (line {stream.LineOf(position)})");
            }

            var arguments = new List<Pattern>();
            while (!stream.IsAt(position, TokenType.Assign))
            {
                var argumentParsed = PatternParser.ParsePattern(stream, position);
                if (!argumentParsed.Success)
                {
                    break;
                }

                arguments.Add(argumentParsed.Value);
                position = argumentParsed.Position;
            }

            var parsedExpression = ExpressionParser.ParseExpression(stream, position + 1, module);
            if (!parsedExpression.Success)
            {
                throw new ParserException($"No expression found for function at line { stream.LineOf(position) }");
            }

            var function = new Function(name, arguments.ToArray(), parsedExpression.Value);
            return new ParseResult<Function>(true, function, position);
        }
    }
}
