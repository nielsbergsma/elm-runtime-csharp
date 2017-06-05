using ElmRuntime2.Exceptions;
using ElmRuntime2.Expressions;
using ElmRuntime2.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Values
{
    public class Boolean : Value
    {
        private readonly bool value;

        public Boolean(bool value)
        {
            this.value = value;
        }

        public bool Value
        {
            get { return value; }
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            return this;
        }

        public Value Op(Operator @operator)
        {
            throw new RuntimeException($"Unknown operation for boolean {@operator}");
        }

        public Value Op(Operator @operator, Value argument)
        {
            if (!(argument is Boolean))
            {
                throw new RuntimeException("Incompatible types");
            }

            var other = argument as Boolean;
            switch (@operator)
            {
                case Operator.Equal:
                    return new Boolean(value == other.value);

                case Operator.NotEqual:
                    return new Boolean(value != other.value);

                case Operator.And:
                    return new Boolean(value && other.value);

                case Operator.Or:
                    return new Boolean(value || other.value);
            }

            throw new RuntimeException($"Unknown operation for boolean {@operator}");
        }

        public bool SameAs(Value other)
        {
            var otherBoolean = other as Boolean;

            return otherBoolean != null
                && otherBoolean.value == value;
        }
    }
}
