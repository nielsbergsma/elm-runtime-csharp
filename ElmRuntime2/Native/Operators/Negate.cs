using ElmRuntime2.Expressions;
using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Native.Operators
{
    public class Negate : Function
    {
        public Negate()
            : base("―", new Pattern[] { new UnderscorePattern() }, null)
        {
        }

        public override Expression Evaluate(Scope scope, Expression[] argumentValues)
        {
            var value = argumentValues[0];
            if (value is Integer)
            {
                return new Integer(-(value as Integer).AsInt());
            }
            else
            {
                return new Float(-(value as Float).AsFloat());
            }
        }
    }
}
