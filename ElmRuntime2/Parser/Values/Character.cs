using ElmRuntime2.Exceptions;
using ElmRuntime2.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser.Values
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

        public Expression Evaluate(Value[] arguments, Scope scope)
        {
            return this;
        }

        public Value Op(Operator @operator)
        {
            throw new RuntimeException($"Unknown operation for char {@operator}");
        }

        public Value Op(Operator @operator, Value argument)
        {
            if (!(argument is Character))
            {
                throw new RuntimeException("Incompatible types");
            }

            var other = argument as Character;
            switch (@operator)
            {
                case Operator.Equal:
                    return new Boolean(value == other.value);

                case Operator.NotEqual:
                    return new Boolean(value != other.value);
            }

            throw new RuntimeException($"Unknown operation for char {@operator}");
        }

        public bool SameAs(Value other)
        {
            var otherChar = other as Character;

            return otherChar != null
                && otherChar.value == value;
        }
    }
}
