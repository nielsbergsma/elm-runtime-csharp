using ElmRuntime2.Exceptions;
using ElmRuntime2.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser.Values
{
    public class List : Value
    {
        private readonly Value[] values;

        public List(Value[] values)
        {
            this.values = values;
        }

        public Value[] Values
        {
            get { return values; }
        }

        public Expression Evaluate(Value[] arguments, Scope scope)
        {
            return this;
        }

        public bool IsEmpty()
        {
            return values.Length == 0;
        }

        public Value Head()
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

        public Value Op(Operator @operator)
        {
            throw new RuntimeException($"Unknown operation for list {@operator}");
        }

        public Value Op(Operator @operator, Value argument)
        {
            switch (@operator)
            {
                case Operator.Prepend:
                    return Prepend(argument);

                case Operator.Concat:
                    if (!(argument is List))
                    {
                        throw new RuntimeException("Concat requires a list");
                    }
                    return Concat(argument as List);

                case Operator.Equal:
                    return new Boolean(SameAs(argument));

                case Operator.NotEqual:
                    return new Boolean(!SameAs(argument));
            }

            throw new RuntimeException($"Unknown operation for list {@operator}");
        }

        public bool SameAs(Value other)
        {
            var otherList = other as List;
            if (otherList == null || otherList.values.Length != values.Length)
            {
                return false;
            }

            for (var v = 0; v < values.Length; v++)
            {
                if (!otherList.values[v].SameAs(values[v]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
