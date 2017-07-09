using ElmRuntime2.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Native.Operators
{
    public class RightCombinator : Function
    {
        public RightCombinator()
            : base(">>", new Pattern[] { new UnderscorePattern(), new UnderscorePattern() }, null)
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

            return new Function(
                "<composition>",
                new[] { new VariablePattern("_arg") },
                new Call(right, new Call(left, new Call("_arg")))
            );
        }
    }
}
