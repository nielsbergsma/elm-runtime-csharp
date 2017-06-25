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
        private readonly Pattern[] arguments;
        private readonly Expression expression;

        public Function(string name, Pattern[] arguments, Expression expression)
        {
            this.name = name;
            this.arguments = arguments;
            this.expression = expression;
        }

        public string Name
        {
            get { return name; }
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            var expressionScope = new Scope(scope);

            //bring arguments into scope
            for (var a = 0; a < arguments.Length && a < this.arguments.Length; a++)
            {
                this.arguments[a].Evaluate(new Expression[] { arguments[a] }, expressionScope);
            }

            //curry function
            if (arguments.Length < this.arguments.Length)
            {
                var newArguments = new List<Pattern>();
                for (var a = arguments.Length; a < this.arguments.Length; a++)
                {
                    newArguments.Add(this.arguments[a]);
                }

                return new Lambda(expressionScope.Unwrap(), newArguments.ToArray(), expression);
            }

            //evaluate
            return expression.Evaluate(arguments, expressionScope);
        }

        public static ParseResult<Function> Parse(TokenStream stream, int position, Module module)
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
                arguments.Add(argumentParsed.Value);
                position = argumentParsed.Position;
            }
                      
            var parsedExpression = ExpressionParser.Parse(stream, position + 1, module);
            if (!parsedExpression.Success)
            {
                throw new ParserException($"No expression found for function at line { stream.LineOf(position) }");
            }

            var function = new Function(name, arguments.ToArray(), parsedExpression.Value);
            return new ParseResult<Function>(true, function, position);
        }
    }
}
