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
    public static class ExpressionParser
    {
        public static ParseResult<Expression> Parse(TokenStream stream, int position)
        {
            if (position >= stream.Length)
            {
                return new ParseResult<Expression>(false, default(Expression), position);
            }

            //basic values
            if (stream.IsAt(position, TokenType.Int))
            {
                var value = int.Parse(stream.At(position).Content);
                return new ParseResult<Expression>(true, new Values.Integer(value), position + 1);
            }
            else if (stream.IsAt(position, TokenType.Float))
            {
                var value = float.Parse(stream.At(position).Content);
                return new ParseResult<Expression>(true, new Values.Float(value), position + 1);
            }
            else if (stream.IsAt(position, TokenType.Char))
            {
                var value = stream.At(position).Content[0];
                return new ParseResult<Expression>(true, new Values.Character(value), position + 1);
            }
            else if (stream.IsAt(position, TokenType.String))
            {
                var value = stream.At(position).Content;
                return new ParseResult<Expression>(true, new Values.String(value), position + 1);
            }
            else if (stream.IsAt(position, TokenType.True))
            {
                return new ParseResult<Expression>(true, new Values.Boolean(true), position + 1);
            }
            else if (stream.IsAt(position, TokenType.False))
            {
                return new ParseResult<Expression>(true, new Values.Boolean(false), position + 1);
            }
            //list 
            else if (stream.IsAt(position, TokenType.LeftBracket))
            {
                var parsed = ParserHelper.ParseList(stream, position);
                var isRange = parsed.Value.Length == 1 && parsed.Value[0].ContainsInExpression(0, TokenType.Range);

                if (isRange)
                {
                    var parsedRange = ListRange.Parse(stream, position);
                    if (!parsedRange.Success)
                    {
                        throw new ParserException($"Unable to parse range near line {stream.LineOf(position)}");
                    }

                    return new ParseResult<Expression>(true, parsedRange.Value, parsed.Position);
                }
                else
                {
                    var parsedList = ListConstruct.Parse(stream, position);
                    if (!parsedList.Success)
                    {
                        throw new ParserException($"Unable to parse list near line {stream.LineOf(position)}");
                    }

                    return new ParseResult<Expression>(true, parsedList.Value, parsed.Position);
                }
            }
            //record
            else if (stream.IsAt(position, TokenType.LeftBrace))
            {
                var parsed = ParserHelper.ParseList(stream, position);
                var isUpdate = stream.IsAt(position, TokenType.LeftBrace, TokenType.Identifier, TokenType.Pipe);

                if (isUpdate)
                {
                    var recordUpdate = RecordUpdate.Parse(stream, position);
                    if (!recordUpdate.Success)
                    {
                        throw new ParserException($"Unable to parse record near line {stream.LineOf(position)}");
                    }

                    return new ParseResult<Expression>(true, recordUpdate.Value, parsed.Position);
                }
                else
                {
                    var recordConstruct = RecordConstruct.Parse(stream, position);
                    if (!recordConstruct.Success)
                    {
                        throw new ParserException($"Unable to parse record near line {stream.LineOf(position)}");
                    }

                    return new ParseResult<Expression>(true, recordConstruct.Value, parsed.Position);
                }
            }

            return new ParseResult<Expression>(false, default(Expression), 0);
        }
    }
}
