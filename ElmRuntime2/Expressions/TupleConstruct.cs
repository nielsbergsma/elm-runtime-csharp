using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElmRuntime2.Values;
using ElmRuntime2.Exceptions;
using ElmRuntime2.Parser;
using ElmRuntime2.Lexer;

namespace ElmRuntime2.Expressions
{
    public class TupleConstruct : Expression
    {
        private readonly Expression[] expressions;

        public TupleConstruct(Expression[] expressions)
        {
            this.expressions = expressions;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            var values = new List<Value>();
            foreach(var expression in expressions)
            {
                var result = expression.Evaluate(arguments, scope);
                if (!(result is Value))
                {
                    throw new RuntimeException("Tuple expression must be evaluated to a value");
                }
                values.Add(result as Value);
            }

            return new Values.Tuple(values.ToArray());
        }

        public static ParseResult<TupleConstruct> Parse(TokenStream stream, int position, Module module)
        {
            if (stream.IsAt(position, TokenType.LeftParen, TokenType.Comma))
            {
                var numberOfElements = 0;
                for (; position < stream.Length && !stream.IsAt(position, TokenType.RightParen); position++)
                {
                    numberOfElements++;
                }

                if (stream.IsAt(position, TokenType.RightParen))
                {
                    position++;
                }

                var expressions = new List<Expression>();
                for (var e = 0; e < numberOfElements; e++)
                {
                    var expression = ExpressionParser.Parse(stream, position, module);
                    if (expression.Success)
                    {
                        expressions.Add(expression.Value);
                    }
                    else
                    {
                        throw new ParserException($"Unable to parse expression in tuple near line ${stream.LineOf(position)}");
                    }
                    position = expression.Position;
                }

                var tupleConstruct = new TupleConstruct(expressions.ToArray());
                return new ParseResult<TupleConstruct>(true, tupleConstruct, position);
            }

            var array = ParserHelper.ParseArray(stream, position);
            if (array.Value.Length > 1)
            {
                var expressions = new List<Expression>();
                foreach (var element in array.Value)
                {
                    var expression = ExpressionParser.Parse(element, 0, module);
                    if (expression.Success)
                    {
                        expressions.Add(expression.Value);
                    }
                    else
                    {
                        throw new ParserException($"Unable to parse expression in tuple near line ${stream.LineOf(position)}");
                    }
                }

                var tupleConstruct = new TupleConstruct(expressions.ToArray());
                return new ParseResult<TupleConstruct>(true, tupleConstruct, array.Position);
            }

            throw new ParserException($"Unable to parse tuple expression near line {stream.LineOf(position)}");
        }
    }
}
