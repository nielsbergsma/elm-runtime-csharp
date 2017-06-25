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
    public class Union : Value
    {
        private readonly string constructor;
        private readonly Expression[] values;

        public Union(string constructor, Expression[] values)
        {
            this.constructor = constructor;
            this.values = values;
        }


        public string Constructor
        {
            get { return constructor; }
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            return this;
        }

        public bool TryGet(int index, out Expression value)
        {
            if (index < values.Length)
            {
                value = values[index];
                return true;
            }
            else
            {
                value = default(Expression);
                return false;
            }
        }

        public bool OperatorEquals(Expression op2)
        {
            var other = op2 as Union;
            if (other == null || other.values.Length != values.Length)
            {
                return false;
            }

            for (var v = 0; v < values.Length; v++)
            {
                var thisValue = this.values[v] as Value;
                var thatValue = other.values[v] as Value;

                if (thisValue == null || thatValue == null || !thisValue.OperatorEquals(thatValue))
                {
                    return false;
                }
            }

            return true;
        }

        public bool OperatorLesserThan(Expression op2)
        {
            return false;
        }
    }
}
