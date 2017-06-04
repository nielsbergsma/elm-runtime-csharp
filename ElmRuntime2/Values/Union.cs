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
        private readonly Value[] values;

        public Union(string constructor, Value[] values)
        {
            this.constructor = constructor;
            this.values = values;
        }

        public Expression Evaluate(Value[] arguments, Scope scope)
        {
            return this;
        }

        public Value Get(int index)
        {
            return index < values.Length ? values[index] : null;
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
            var otherUnion = other as Union;
            if (otherUnion == null || otherUnion.constructor == constructor || otherUnion.values.Length != values.Length)
            {
                return false;
            }

            for (var v = 0; v < values.Length; v++)
            {
                if (!otherUnion.values[v].SameAs(values[v]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
