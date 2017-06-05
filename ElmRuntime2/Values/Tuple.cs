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
        private readonly Value[] values;

        public Tuple(Value[] values)
        {
            this.values = values;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            return this;
        }

        public bool TryGet(int item, out Value value)
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

        public Value Op(Operator @operator)
        {
            throw new RuntimeException($"Unknown operation for tuple {@operator}");
        }

        public Value Op(Operator @operator, Value argument)
        {
            switch (@operator)
            {
                case Operator.Equal:
                    return new Boolean(SameAs(argument));

                case Operator.NotEqual:
                    return new Boolean(!SameAs(argument));
            }

            throw new RuntimeException($"Unknown operation for tulpe {@operator}");
        }

        public bool SameAs(Value other)
        {
            var otherTuple = other as Tuple;
            if (otherTuple == null || otherTuple.values.Length != values.Length)
            {
                return false;
            }

            for (var v = 0; v < values.Length; v++)
            {
                if (!otherTuple.values[v].SameAs(values[v]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
