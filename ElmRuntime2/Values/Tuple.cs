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
    public class Tuple : Value
    {
        private readonly Expression[] values;

        public Tuple(params Value[] values)
        {
            this.values = values;
        }

        public Expression Evaluate(Scope scope)
        {
            return this;
        }

        public Expression[] Values
        {
            get { return values; }
        }

        public bool TryGet(int item, out Expression value)
        {
            if (item < values.Length)
            {
                value = values[item];
                return true;
            }
            else
            {
                value = default(Value);
                return false;
            }
        }

        public bool OperatorEquals(Expression op2)
        {
            var other = op2 as Tuple;
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
            var other = op2 as Tuple;
            if (other == null || other.values.Length != values.Length)
            {
                return false;
            }

            for (var v = 0; v < values.Length; v++)
            {
                var thisValue = this.values[v] as Value;
                var thatValue = other.values[v] as Value;

                if (thisValue == null || thatValue == null)
                {
                    return false;
                }
                else if (thisValue.OperatorEquals(thatValue))
                {
                    continue;
                }
                else if (thisValue.OperatorLesserThan(thatValue))
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            return $"({string.Join(",", values.Select(v => v.ToString()))})";
        }
    }
}
