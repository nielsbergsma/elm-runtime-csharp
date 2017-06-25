using ElmRuntime2.Exceptions;
using ElmRuntime2.Expressions;
using ElmRuntime2.Lexer;
using ElmRuntime2.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Values
{
    public class String : Value
    {
        private readonly string value;

        public String(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            return this;
        }

        public bool OperatorEquals(Expression op2)
        {
            var other = op2 as String;
            return other != null && other.value == value;
        }

        public bool OperatorLesserThan(Expression op2)
        {
            var other = op2 as String;
            return other != null && string.Compare(value, (op2 as String).value) < 0;
        }
    }
}
