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
        public static ParseResult<Expression> ParseExpression(TokenStream stream, int position, Module module)
        {
            if (position >= stream.Length)
            {
                return new ParseResult<Expression>(false, default(Expression), position);
            }

            //resolve operators
            var start = position;
            var end = stream.SkipToNextExpression(position);
            stream = OperatorParser.Resolve(stream, position, module);
            position = 0;

            //basic values
            if (stream.IsAnyAt(position, TokenType.True, TokenType.False, TokenType.Int, TokenType.Float, TokenType.String, TokenType.Char))
            {
                var parsed = ValueParser.ParseValue(stream, position);
                return new ParseResult<Expression>(parsed.Value, start + parsed.Position);
            }
            //list 
            else if (stream.IsAt(position, TokenType.LeftBracket))
            {
                var parsedList = ListParser.ParseList(stream, position, module);
                if (!parsedList.Success)
                {
                    throw new ParserException($"Unable to parse list near line {stream.LineOf(position)}");
                }
            }
            //record
            else if (stream.IsAt(position, TokenType.LeftBrace))
            {
                var parsed = ParserHelper.ParseArray(stream, position);
                var isUpdate = stream.IsAt(position, TokenType.LeftBrace, TokenType.Identifier, TokenType.Pipe);

                if (isUpdate)
                {
                    var recordUpdate = RecordParser.ParseRecordUpdate(stream, position, module);
                    if (!recordUpdate.Success)
                    {
                        throw new ParserException($"Unable to parse record near line {stream.LineOf(position)}");
                    }

                    return new ParseResult<Expression>(true, recordUpdate.Value, start + parsed.Position);
                }
                else
                {
                    var recordConstruct = RecordParser.ParseRecordConstruct(stream, position, module);
                    if (!recordConstruct.Success)
                    {
                        throw new ParserException($"Unable to parse record near line {stream.LineOf(position)}");
                    }

                    return new ParseResult<Expression>(true, recordConstruct.Value, start + parsed.Position);
                }
            }
            //tuple, parentheses
            else if (stream.IsAt(position, TokenType.LeftParen))
            {
                var parsed = ParserHelper.ParseArray(stream, position);

                //tuple
                if (stream.IsAt(position, TokenType.LeftParen, TokenType.Comma) || parsed.Value.Length > 1)
                {
                    var tupleConstruct = TupleParser.ParseTuple(stream, position, module);
                    if (!tupleConstruct.Success)
                    {
                        throw new ParserException($"Unable to parse tuple near line {stream.LineOf(position)}");
                    }
                    return new ParseResult<Expression>(true, tupleConstruct.Value, start + parsed.Position);
                }
                //parentheses
                else if (parsed.Value.Length == 1)
                {
                    var groupParsed = ParseExpression(parsed.Value[0], 0, module);
                    var group = new Group(groupParsed.Value);
                    return new ParseResult<Expression>(true, group, start + parsed.Position);
                }
            }
            //member access as lambda
            else if (stream.IsAt(position, TokenType.Dot, TokenType.Identifier))
            {
                var field = stream.At(position + 1).Content;
                var fieldAccess = new FieldAccess(field);
                return new ParseResult<Expression>(true, fieldAccess, start + 2);
            }
            //lambda
            else if (stream.IsAt(position, TokenType.Backslash))
            {
                var arguments = new List<Pattern>();
                for (position++; position < stream.Length && !stream.IsAt(position, TokenType.Arrow);)
                {
                    var argumentParsed = PatternParser.ParsePattern(stream, position);
                    if (!argumentParsed.Success)
                    {
                        break;
                    }
                    arguments.Add(argumentParsed.Value);
                    position = argumentParsed.Position;
                }

                if (!stream.IsAt(position, TokenType.Arrow))
                {
                    throw new ParserException($"Unexpected token while parsing lambda expression");
                }
                position++;

                var expressionParsed = ParseExpression(stream, position, module);
                if (!expressionParsed.Success)
                {
                    throw new ParserException($"No expression for lambda expression");
                }

                var lambda = new LambdaConstruct(arguments.ToArray(), expressionParsed.Value);
                return new ParseResult<Expression>(true, lambda, start + expressionParsed.Position);
            }
            //case pattern
            else if (stream.IsAt(position, TokenType.Case))
            {
                var subjectParsed = ParseExpression(stream, position + 1, module);
                var subject = subjectParsed.Value;
                position = subjectParsed.Position;

                if (!stream.IsAt(position, TokenType.Of))
                {
                    throw new ParserException($"Unexpected token while parsing case expression");
                }
                position++;

                var column = stream.At(position).Column;
                var patterns = new List<CasePattern>();
                while (position < stream.Length && column <= stream.At(position).Column)
                {
                    var conditionParsed = PatternParser.ParsePattern(stream, position);
                    position = conditionParsed.Position;

                    if (!stream.IsAt(position, TokenType.Arrow))
                    {
                        throw new ParserException($"Unexpected token while parsing case expression");
                    }

                    var expressionParsed = ParseExpression(stream, position + 1, module);
                    position = expressionParsed.Position;

                    patterns.Add(new CasePattern(conditionParsed.Value, expressionParsed.Value));
                }

                var @case = new Case(subject, patterns.ToArray());
                return new ParseResult<Expression>(@case, start + position);
            }
            else if (stream.IsAt(position, TokenType.Let))
            {
                var parsed = LetParser.ParseLet(stream, position, module);
                return new ParseResult<Expression>(parsed.Success, parsed.Value, parsed.Position);
            }
            else if (stream.IsAt(position, TokenType.If))
            {
                var condition = ParseExpression(stream, position + 1, module);
                if (!stream.IsAt(condition.Position, TokenType.Then))
                {
                    throw new ParserException($"Unable to parse if expression, cannot find then");
                }

                var then = ParseExpression(stream, condition.Position + 1, module);
                if (!stream.IsAt(then.Position, TokenType.Else))
                {
                    throw new ParserException($"Unable to parse if expression, cannot find else");
                }

                var @else = ParseExpression(stream, then.Position + 1, module);
                if (!@else.Success)
                {
                    throw new ParserException($"Unexpected end of if expression");
                }

                var @if = new If(condition.Value, then.Value, @else.Value);
                return new ParseResult<Expression>(@if, start + @else.Position);
            }
            else if (stream.IsAt(position, TokenType.Identifier))
            {
                var name = stream.At(position).Content;

                ////local variable?
                //var argumentsStart = position + 1;
                //var argumentsEnd = stream.SkipToNextExpression(position);
                //var arguments = new List<Expression>();

                //for (var argumentPosition = argumentsStart; argumentPosition < argumentsEnd;)
                //{
                //    var argument = Parse(stream, argumentPosition, module);
                //    if (argument.Success)
                //    {
                //        arguments.Add(argument.Value);
                //    }
                //    argumentPosition = argument.Position;
                //}

                var invocation = new Invocation(name, new Expression[0]);
                return new ParseResult<Expression>(true, invocation, start + position + 1);
            }

            return new ParseResult<Expression>(false, default(Expression), 0);
        }
    }
}
