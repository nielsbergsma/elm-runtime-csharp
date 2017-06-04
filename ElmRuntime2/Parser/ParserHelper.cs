using ElmRuntime2.Exceptions;
using ElmRuntime2.Expressions;
using ElmRuntime2.Lexer;
using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser
{
    public static class ParserHelper
    {
        public static ParseResult<TokenStream[]> ParseList(TokenStream stream, int position)
        {
            var list = new List<TokenStream>();
            var seperator = TokenType.Comma;
            var stop = TokenType.RightParen;

            if (position >= stream.Length)
            {
                return new ParseResult<TokenStream[]>(false, new TokenStream[0], position);
            }

            switch(stream.At(position).Type)
            {
                case TokenType.LeftParen:
                    stop = TokenType.RightParen;
                    break;

                case TokenType.LeftBrace:
                    stop = TokenType.RightBrace;
                    break;

                case TokenType.LeftBracket:
                    stop = TokenType.RightBracket;
                    break;

                default:
                    return new ParseResult<TokenStream[]>(false, new TokenStream[0], position);
            }

            var nesting = 0;
            var stack = new List<Token>();

            for (position++; position < stream.Length; position++)
            {
                if (stream.IsAt(position, seperator) && nesting == 0)
                {
                    if (stack.Any())
                    {
                        list.Add(new TokenStream(stack.ToArray()));
                        stack.Clear();
                    }
                }
                else if (stream.IsAt(position, stop) && nesting == 0)
                {
                    if (stack.Any())
                    {
                        list.Add(new TokenStream(stack.ToArray()));
                        stack.Clear();
                    }

                    position++;
                    break;
                }
                else
                {
                    stack.Add(stream.At(position));
                    if (stream.IsAnyAt(position, TokenType.LeftParen, TokenType.LeftBrace, TokenType.LeftBracket))
                    {
                        nesting++;
                    }
                    else if (stream.IsAnyAt(position, TokenType.RightParen, TokenType.RightBrace, TokenType.RightBracket))
                    {
                        nesting--;
                    }
                }
            }

            return new ParseResult<TokenStream[]>(list.ToArray(), position);
        }
        
        public static ParseResult<Expression> ParseLine(TokenStream stream, int position)
        {
            if (position >= stream.Length)
            {
                return new ParseResult<Expression>(false, default(Expression), position);
            }

            if (!stream.AtStartOfExpression(position))
            {
                throw new ParserException($"Not at start of line at { stream.At(position).Line + 1 }");
            }

            //type alias (ignore?)
            if (stream.IsAt(position, TokenType.TypeDef, TokenType.Alias))
            {
                var nextExpressionStart = stream.SkipToNextExpression(position);
                return new ParseResult<Expression>(false, default(Expression), nextExpressionStart);
            }
            //type definition (ignore?)
            else if (stream.IsAt(position, TokenType.TypeDef))
            {
                var nextExpressionStart = stream.SkipToNextExpression(position);
                return new ParseResult<Expression>(false, default(Expression), nextExpressionStart);
            }
            //port (ignore)
            else if (stream.IsAt(position, TokenType.Port))
            {
                var nextExpressionStart = stream.SkipToNextExpression(position);
                return new ParseResult<Expression>(false, default(Expression), nextExpressionStart);
            }
            //annotation (ignore)
            else if (stream.ContainsInExpression(position, TokenType.Colon))
            {
                var nextExpressionStart = stream.SkipToNextExpression(position);
                return new ParseResult<Expression>(false, default(Expression), nextExpressionStart);
            }
            //operator
            else if (stream.IsAt(position, TokenType.LeftParen) && stream.ContainsInExpression(position + 1, TokenType.RightParen))
            {
                throw new NotImplementedException();
            }
            //set association + precedence of operator
            else if (stream.IsAnyAt(position, TokenType.Infix, TokenType.Infixl, TokenType.Infixr))
            {
                throw new NotImplementedException();
            }
            //function expression (named expression)
            else if (IsVariableName(stream, position) && stream.ContainsInExpression(position, TokenType.Assign))
            {
                var parsed = Function.Parse(stream, position);
                var nextExpressionStart = stream.SkipToNextExpression(position);

                if (parsed.Success)
                {
                    return new ParseResult<Expression>(true, parsed.Value, nextExpressionStart);
                }
                else
                {
                    return new ParseResult<Expression>(false, default(Expression), nextExpressionStart);
                }
            }
            else
            {
                throw new ParserException($"Encountered unknown expression at { stream.At(position).Line + 1 }");
            }
        }

        public static bool IsVariableName(TokenStream stream, int position)
        {
            if (stream.IsAt(position, TokenType.Identifier))
            {
                var content = stream.At(position).Content;
                return content.Length > 0 && char.IsLower(content[0]);
            }

            return false;
        }
    }
}
