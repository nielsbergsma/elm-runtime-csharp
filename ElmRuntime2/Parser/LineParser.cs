﻿using ElmRuntime2.Exceptions;
using ElmRuntime2.Expressions;
using ElmRuntime2.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser
{
    public static class LineParser
    {
        public static ParseResult<Expression> Parse(TokenStream stream, int position)
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
            else if (ParserHelper.IsVariableName(stream, position) && stream.ContainsInExpression(position, TokenType.Assign))
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
    }
}