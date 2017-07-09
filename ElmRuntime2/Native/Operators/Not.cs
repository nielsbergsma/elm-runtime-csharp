using ElmRuntime2.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Native.Operators
{
    public class Not : Function
    {
        public Not()
            : base("not", new Pattern[] { new UnderscorePattern() }, null)
        {
        }

        public override Expression Evaluate(Scope scope, Expression[] argumentValues)
        {
            if (argumentValues.Length < arguments.Length)
            {
                return Curry(scope, argumentValues);
            }

            var argument = argumentValues[0] as Values.Boolean;

            return new Values.Boolean(argument != null && !argument.Value);
        }
    }
}
