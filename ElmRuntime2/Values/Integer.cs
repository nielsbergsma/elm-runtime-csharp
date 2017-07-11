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
    public class Integer : Number
    {
        private int value;

        public Integer(int value)
        {
            this.value = value;
        }

        public float AsFloat()
        {
            return value;
        }

        public int AsInt()
        {
            return value;
        }

        public Expression Evaluate(Scope scope)
        {
            return this;
        }

        public bool OperatorEquals(Expression op2)
        {
            return op2 is Integer && (op2 as Integer).value == value;
        }

        public bool OperatorLessThan(Expression op2)
        {
            return op2 is Integer && value < (op2 as Integer).value;
        }

        public Float ToFloat()
        {
            return new Float(value);
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public string ToJson()
        {
            return value.ToString();
        }
    }
}
