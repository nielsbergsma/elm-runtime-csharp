using ElmRuntime2.Expressions;
using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Native.Operators
{
    public class Modulo : Function
    {
        public Modulo()
            : base("%", new Pattern[] { new UnderscorePattern(), new UnderscorePattern() }, null)
        {
        }

        public override Expression Evaluate(Scope scope, Expression[] argumentValues)
        {
            if (argumentValues.Length < arguments.Length)
            {
                return Curry(scope, argumentValues);
            }

            var left = argumentValues[0] as Number;
            var right = argumentValues[1] as Number;

            var result = left.AsInt() % right.AsInt();
            return new Integer(result);
        }
    }
}
