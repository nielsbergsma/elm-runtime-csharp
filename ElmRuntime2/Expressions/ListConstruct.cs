using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElmRuntime2.Exceptions;
using ElmRuntime2.Lexer;
using ElmRuntime2.Values;
using ElmRuntime2.Parser;

namespace ElmRuntime2.Expressions
{
    public class ListConstruct : Expression
    {
        private readonly Expression[] expressions;

        public ListConstruct(Expression[] expressions)
        {
            this.expressions = expressions;
        }

        public Expression Evaluate(Value[] arguments, Scope scope)
        {
            var values = new List<Value>();
            foreach(var expression in expressions)
            {
                var value = expression.Evaluate(arguments, scope) as Value;
                values.Add(value as Value);
            }

            return new List(values.ToArray());
        }

        public static ParseResult<ListConstruct> Parse(TokenStream stream, int position)
        {
            var parsed = ParserHelper.ParseList(stream, position);
            if (!parsed.Success)
            {
                return new ParseResult<ListConstruct>(false, default(ListConstruct), parsed.Position);
            }

            var expressions = new List<Expression>();
            foreach (var tokens in parsed.Value)
            {
                var expression = ExpressionParser.ParseExpression(tokens, 0);
                if (!expression.Success)
                {
                    throw new ParserException($"Unable to parse list item expression near line {stream.LineOf(position)}");
                }
                expressions.Add(expression.Value);
            }

            var construction = new ListConstruct(expressions.ToArray());
            return new ParseResult<ListConstruct>(true, construction, parsed.Position);
        }
    }
}
