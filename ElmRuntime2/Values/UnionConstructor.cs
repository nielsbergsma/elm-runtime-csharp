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
    public class UnionConstructor : Value
    {
        private readonly string constructor;
        private readonly Expression[] values;

        public UnionConstructor(string constructor, params Expression[] values)
        {
            this.constructor = constructor;
            this.values = values;
        }


        public string Constructor
        {
            get { return constructor; }
        }

        public Expression Evaluate(Scope scope)
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
            var other = op2 as UnionConstructor;
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

        public bool OperatorLessThan(Expression op2)
        {
            return false;
        }

        public override string ToString()
        {
            return $"<{constructor} {string.Join(",", values.Select(v => v.ToString()))}>";
        }
    }
}
