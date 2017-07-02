using ElmRuntime2.Exceptions;
using ElmRuntime2.Expressions;
using ElmRuntime2.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Values
{
    public class Boolean : Value
    {
        private readonly bool value;

        public Boolean(bool value)
        {
            this.value = value;
        }

        public bool Value
        {
            get { return value; }
        }

        public Expression Evaluate(Scope scope)
        {
            return this;
        }

        public bool OperatorEquals(Expression op2)
        {
            return op2 is Boolean && (op2 as Boolean).value == value;
        }

        public bool OperatorLesserThan(Expression op2)
        {
            return false;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
