using ElmRuntime2.Exceptions;
using ElmRuntime2.Lexer;
using ElmRuntime2.Parser;
using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public class Function : Expression
    {
        private readonly string name;
        private readonly FunctionArgument[] arguments;
        private readonly Expression expression;

        public Function(string name, FunctionArgument[] arguments, Expression expression)
        {
            this.name = name;
            this.arguments = arguments;
            this.expression = expression;
        }

        public string Name
        {
            get { return name; }
        }

        public Expression Evaluate(Value[] arguments, Scope scope)
        {
            var expressionScope = new Scope(scope);

            //bring arguments into scope
            for (var a = 0; a < arguments.Length && a < this.arguments.Length; a++)
            {
                this.arguments[a].SetScope(expressionScope, arguments[a]);
            }

            //curry function
            if (arguments.Length < this.arguments.Length)
            {
                var newArguments = new List<FunctionArgument>();
                for (var a = arguments.Length; a < this.arguments.Length; a++)
                {
                    newArguments.Add(this.arguments[a]);
                }

                return new Lambda(expressionScope.Unwrap(), newArguments.ToArray(), expression);
            }

            //evaluate
            return expression.Evaluate(arguments, expressionScope);
        }

        public static ParseResult<Function> Parse(TokenStream stream, int position)
        {
            if (!stream.IsAt(position, TokenType.Identifier))
            {
                throw new ParserException($"Expected idenfier start function (line {stream.LineOf(position)})");
            }

            var name = stream.At(position).Content;

            var arguments = new List<FunctionArgument>();
            for(position++; position < stream.Length && !stream.IsAt(position, TokenType.Assign);)
            {
                //regular argument
                if (stream.IsAt(position, TokenType.Identifier))
                {
                    var argumentName = stream.At(position).Content;
                    arguments.Add(new FunctionNamedArgument(argumentName));
                    position++;
                }
                //deconstructive tuple
                else if (stream.IsAt(position, TokenType.LeftParen))
                {
                    var parsed =  ParserHelper.ParseList(stream, position);
                    var names = new List<string>();

                    foreach(var argument in parsed.Value)
                    {
                        if (argument.IsAt(0, TokenType.Identifier))
                        {
                            names.Add(argument.At(0).Content);
                        }
                    }

                    arguments.Add(new FunctionDeconstructiveTupleArgument(names.ToArray()));
                    position = parsed.Position;
                }
                //deconstructive record
                else if (stream.IsAt(position, TokenType.LeftBrace))
                {
                    var parsed = ParserHelper.ParseList(stream, position);
                    var names = new List<string>();

                    foreach (var argument in parsed.Value)
                    {
                        if (argument.IsAt(0, TokenType.Identifier))
                        {
                            names.Add(argument.At(0).Content);
                        }
                    }

                    arguments.Add(new FunctionDeconstructiveRecordArgument(names.ToArray()));
                    position = parsed.Position;
                }
                else
                {
                    throw new ParserException($"Unsupported function argument encountered at line {stream.LineOf(position)}");
                }
            }

            var parsedExpression = ExpressionParser.Parse(stream, position + 1);
            if (!parsedExpression.Success)
            {
                throw new ParserException($"No expression found for function at line { stream.LineOf(position) }");
            }

            var function = new Function(name, arguments.ToArray(), parsedExpression.Value);
            return new ParseResult<Function>(true, function, position);
        }
    }
}
