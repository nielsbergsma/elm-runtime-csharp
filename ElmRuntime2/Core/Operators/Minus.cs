using ElmRuntime2.Expressions;
using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Core.Operators
{
    public class Minus : Expression
    {
        private readonly Expression curry;

        public static Function AsFunction()
        {
            return new Function("+", new Pattern[0], new Minus(null));
        }

        private Minus(Expression curry)
        {
            this.curry = curry;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            if (arguments.Length == 0)
            {
                return this;
            }
            else if (arguments.Length == 1 && curry == null)
            {
                return new Minus(arguments[0]);
            }

            var left = curry;
            var right = default(Expression);
            if (arguments.Length == 1)
            {
                right = arguments[0].Evaluate(new Expression[0], scope);
            }
            else
            {
                left = arguments[0].Evaluate(new Expression[0], scope);
                right = arguments[1].Evaluate(new Expression[0], scope);
            }

            if (left is Integer && right is Integer)
            {
                var result = (left as Number).AsInt() - (right as Number).AsInt();
                return new Integer(result);
            }
            else
            {
                var result = (left as Number).AsFloat() - (right as Number).AsFloat();
                return new Float(result);
            }
        }
    }
}
