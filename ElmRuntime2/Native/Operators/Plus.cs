using ElmRuntime2.Exceptions;
using ElmRuntime2.Expressions;
using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Native.Operators
{
    public class Plus : Function
    {
        public Plus()
            : base("+", new Pattern[] { new UnderscorePattern(), new UnderscorePattern() }, null)
        {
        }

        public override Expression Evaluate(Scope scope, Expression[] argumentValues)
        {
            if (argumentValues.Length < arguments.Length)
            {
                return Curry(scope, argumentValues);
            }

            var left = argumentValues[0].Evaluate(scope);
            var right = argumentValues[1].Evaluate(scope);

            if (left is Integer && right is Integer)
            {
                var result = (left as Number).AsInt() + (right as Number).AsInt();
                return new Integer(result);
            }
            else
            {
                var result = (left as Number).AsFloat() + (right as Number).AsFloat();
                return new Float(result);
            }
        }
    }
}
