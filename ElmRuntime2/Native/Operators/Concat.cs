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
    public class Concat : Function
    {
        public Concat()
            : base("++", new Pattern[] { new UnderscorePattern(), new UnderscorePattern() }, null)
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

            if (left is List && right is List)
            {
                return (left as List).Concat(right as List);
            }
            else if (left is Values.String && right is Values.String)
            {
                return new Values.String((left as Values.String).Value + (right as Values.String).Value);
            }
            else
            {
                throw new RuntimeException($"Unable to concat values");
            }
        }
    }
}
