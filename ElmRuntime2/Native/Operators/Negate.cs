using ElmRuntime2.Expressions;
using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Native.Operators
{
    public class Negate : Expression
    {
        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            if (arguments.Length == 0)
            {
                return this;
            }

            var value = arguments[0].Evaluate(new Expression[0], scope);

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
