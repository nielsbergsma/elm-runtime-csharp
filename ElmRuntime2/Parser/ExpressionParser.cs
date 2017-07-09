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
            return ParseExpression(stream, position, module, false);
        }

        public static ParseResult<Expression> ParseExpression(TokenStream stream, int position, Module module, bool greedy)
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

            var expression = default(Expression);

            //basic values
            if (stream.IsAnyAt(position, TokenType.True, TokenType.False, TokenType.Int, TokenType.Float, TokenType.String, TokenType.Char))
            {
                var parsed = ValueParser.ParseValue(stream, position);

                expression = parsed.Value;
                position = parsed.Position;
            }
            //list 
            else if (stream.IsAt(position, TokenType.LeftBracket))
            {
                var parsedList = ListParser.ParseList(stream, position, module);
                if (!parsedList.Success)
                {
                    throw new ParserException($"Unable to parse list near line {stream.LineOf(position)}");
                }

                expression = parsedList.Value;
                position = parsedList.Position;
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

                    expression = recordUpdate.Value;
                    position = parsed.Position;
                }
                else
                {
                    var recordConstruct = RecordParser.ParseRecordConstruct(stream, position, module);
                    if (!recordConstruct.Success)
                    {
                        throw new ParserException($"Unable to parse record near line {stream.LineOf(position)}");
                    }

                    expression = recordConstruct.Value;
                    position = parsed.Position;
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

                    expression = tupleConstruct.Value;
                    position = parsed.Position;
                }
                //parentheses
                else if (parsed.Value.Length == 1)
                {
                    var groupParsed = ParseExpression(parsed.Value[0], 0, module, true);

                    if (groupParsed.Value is Function)
                    {
                        expression = groupParsed.Value;
                    }
                    else
                    { 
                        expression = new Group(groupParsed.Value);
                    }

                    position = parsed.Position;
                }
            }
            //member access as lambda
            else if (stream.IsAt(position, TokenType.Dot, TokenType.Identifier))
            {
                var field = stream.At(position + 1).Content;

                expression = new FieldAccess(field);
                position += 2;
            }
            //lambda
            else if (stream.IsAt(position, TokenType.Backslash))
            {
                var functionParsed = FunctionParser.ParseFunction(stream, position, module);
                if (!functionParsed.Success)
                {
                    throw new ParserException($"Unexpected token while parsing lambda expression");
                }

                expression = functionParsed.Value;
                position = functionParsed.Position;
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

                expression = new Case(subject, patterns.ToArray());
            }
            //let expression
            else if (stream.IsAt(position, TokenType.Let))
            {
                var parsed = LetParser.ParseLet(stream, position, module);

                expression = parsed.Value;
                position = parsed.Position;
            }
            //infix operator
            else if (stream.IsAt(position, TokenType.OpInfix))
            {
                var symbol = stream.At(position).Content;

                var termParsed = ParseExpression(stream, position + 1, module, false);
                if (!termParsed.Success)
                {
                    throw new ParserException($"Unexpected token while parsing opeartor expression");
                }

                var negation = symbol == "-" && stream.At(position).Column + 1 == stream.At(position + 1).Column;
                if (negation)
                {
                    expression = new Call("―", termParsed.Value);
                }
                else
                {
                    var @operator = default(Operator);
                    if (!module.TryGetOperator(symbol, out @operator))
                    {
                        throw new ParserException($"Unknown operator {symbol}");
                    }
                    expression = new BinaryOperationCall(@operator, termParsed.Value);
                }
                
                position = termParsed.Position;
            }
            //prefix operator
            else if (stream.IsAt(position, TokenType.OpPrefix))
            {
                var symbol = stream.At(position).Content;
                position++;

                var arguments = new List<Expression>();
                var term1Parsed = ParseExpression(stream, position, module, false);
                if (term1Parsed.Success)
                {
                    arguments.Add(term1Parsed.Value);
                    position = term1Parsed.Position;
                }

                var term2Parsed = ParseExpression(stream, position, module, false);
                if (term2Parsed.Success)
                {
                    arguments.Add(term2Parsed.Value);
                    position = term2Parsed.Position;
                }

                if (arguments.Count == 2)
                {
                    var @operator = default(Operator);
                    if (!module.TryGetOperator(symbol, out @operator))
                    {
                        throw new ParserException($"Unknown operator {symbol}");
                    }
                    expression = new BinaryOperationCall(@operator, 10, arguments.ToArray());
                }
                else
                {
                    expression = new Call(symbol, arguments.ToArray());
                }
            }
            //if expression
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

                expression = new If(condition.Value, then.Value, @else.Value);
                position = @else.Position; 
            }
            //identifier
            else if (stream.IsAt(position, TokenType.Identifier))
            {
                var name = stream.At(position).Content;

                expression = new Call(name);
                position += 1;
            }

            while (greedy && position < stream.Length)
            {
                var termParsed = ParseExpression(stream, position, module, false);
                if (!termParsed.Success)
                {
                    throw new ParserException($"Unexpected end expression");
                }

                if (termParsed.Value is BinaryOperationCall)
                {
                    var @operator = termParsed.Value as BinaryOperationCall;
                    if (IsOperatorCall(module, expression))
                    {
                        var currentOperator = expression as BinaryOperationCall;
                        var root = !(@operator.Precedence > currentOperator.Precedence 
                            || (@operator.Precedence == currentOperator.Precedence && @operator.Associativity == OperatorAssociativity.Right));

                        if (root)
                        {
                            @operator.PrependArgument(expression);
                            expression = @operator;
                        }
                        else
                        {
                            var right = expression as Call;
                            while (IsOperatorCall(module, right.LastArgument))
                            {
                                right = right.LastArgument as Call;
                            }

                            @operator.PrependArgument(right.PopArgument());
                            right.AppendArgument(@operator);
                        }
                    }
                    else
                    {
                        @operator.PrependArgument(expression);
                        expression = @operator;
                    }

                    position = termParsed.Position;
                }
                else if (expression is Function)
                {
                    expression = new Call(expression, termParsed.Value);
                    position = termParsed.Position;
                }
                else if (expression is Call)
                {
                    var call = expression as Call;
                    while (call.HasArguments && call.LastArgument is Call)
                    {
                        call = call.LastArgument as Call;
                    }

                    call.AppendArgument(termParsed.Value);
                    position = termParsed.Position;
                }
                else
                {
                    throw new ParserException($"Unexpected expression");
                }
            }

            return new ParseResult<Expression>(expression != null, expression, start + position);
        }

        public static bool IsOperatorCall(Module module, Expression expression)
        {
            var @operator = default(Operator);
            return expression is Call && module.TryGetOperator((expression as Call).Name, out @operator);
        }      
    }
}
