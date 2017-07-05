using ElmRuntime2.Expressions;
using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Native.Operators
{
    public class Power : Function
    {
        public Power()
            : base("^", new Pattern[] { new UnderscorePattern(), new UnderscorePattern() }, null)
        {
        }

        public override Expression Evaluate(Scope scope, Expression[] argumentValues)
        {
            if (argumentValues.Length < arguments.Length)
            {
                return Curry(scope, argumentValues);
            }

            var left = argumentValues[0];
            var right = argumentValues[1];

            if (left is Integer && right is Integer)
            {
                var result = Math.Pow((left as Number).AsInt(), (right as Number).AsInt());
                return new Integer((int) result);
            }
            else
            {
                var result = Math.Pow((left as Number).AsFloat(), (right as Number).AsFloat());
                return new Float((float) result);
            }
        }
    }
}
