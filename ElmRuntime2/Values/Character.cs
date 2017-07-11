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
    public class Character : Value
    {
        private readonly char value;

        public Character(char value)
        {
            this.value = value;
        }

        public char Value
        {
            get { return value; }
        }

        public Expression Evaluate(Scope scope)
        {
            return this;
        }

        public bool OperatorEquals(Expression op2)
        {
            return op2 is Character && (op2 as Character).value == value;
        }

        public bool OperatorLessThan(Expression op2)
        {
            return op2 is Character && value < (op2 as Character).value;
        }

        public override string ToString()
        {
            return $"'{value.ToString()}'";
        }

        public string ToJson()
        {
            return "\"" + value + "\"";
        }
    }
}
