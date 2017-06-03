using ElmRuntime2.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser.Values
{
    public class String : Value
    {
        private readonly string value;
        private object otherFloat;

        public String(string value)
        {
            this.value = value;
        }

        public Value Op(Operator @operator)
        {
            throw new RuntimeException($"Unknown operation for string {@operator}");
        }

        public Value Op(Operator @operator, Value argument)
        {
            if (!(argument is String))
            {
                throw new RuntimeException("Incompatible types");
            }

            var other = argument as String;
            switch (@operator)
            {
                case Operator.Concat:
                    return new String(value + other.value);

                case Operator.Equal:
                    return new Boolean(value == other.value);

                case Operator.NotEqual:
                    return new Boolean(value != other.value);
            }

            throw new RuntimeException($"Unknown operation for string {@operator}");
        }

        public bool SameAs(Value other)
        {
            var otherString = other as String;

            return otherString != null
                && otherString.value == value;
        }
    }
}
