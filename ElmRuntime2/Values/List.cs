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
    public class List : Value
    {
        private readonly Expression[] values;

        public List(params Expression[] values)
        {
            this.values = values;
        }

        public Expression[] Values
        {
            get { return values; }
        }

        public Expression Evaluate(Scope scope)
        {
            return this;
        }

        public bool IsEmpty()
        {
            return values.Length == 0;
        }

        public Expression Head()
        {
            return values.Length > 0 ? values[0] : null;
        }

        public List Tail()
        {
            if (values.Length <= 1)
            {
                return new List(new Value[0]);
            }
            else
            {
                var tail = new Value[values.Length - 1];
                Array.Copy(values, 1, tail, 0, tail.Length);
                return new List(tail);
            }
        }

        public List Prepend(Value value)
        {
            var newValues = new Value[this.values.Length + 1];
            newValues[0] = value;
            this.values.CopyTo(newValues, 1);
            return new List(this.values);
        }

        public List Concat(List other)
        {
            var newValues = new Value[values.Length + other.values.Length];
            Array.Copy(values, newValues, values.Length);
            Array.Copy(other.values, 0, newValues, values.Length, other.values.Length);
            return new List(newValues);
        }

        public bool OperatorEquals(Expression op2)
        {
            var other = op2 as List;
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
            var other = op2 as List;
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
            return $"[{string.Join(",", values.Select(v => v.ToString()))}]";
        }
    }
}
