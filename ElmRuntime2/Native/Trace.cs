using ElmRuntime2.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Native
{
    public class Trace : Function
    {
        public Trace() 
            : base("trace", new[] { new UnderscorePattern() }, null)
        {
        }

        public override Expression Evaluate(Scope scope, Expression[] argumentValues)
        {
            if (argumentValues.Length == 0)
            {
                return this;
            }

            Console.WriteLine($"[trace] {argumentValues[0]}");

            return argumentValues[0];
        }
    }
}
