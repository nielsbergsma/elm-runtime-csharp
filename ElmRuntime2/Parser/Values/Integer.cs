using ElmRuntime2.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser.Values
{
    public class Integer : Number
    {
        private int value;

        public Integer(int value)
        {
            this.value = value;
        }

        public Value Op(Operator @operator)
        {
            if (@operator == Operator.Minus)
            {
                return new Integer(-value);
            }

            throw new RuntimeException($"Unknown operation for integer {@operator}");
        }

        public Value Op(Operator @operator, Value argument)
        {
            if (!(argument is Number))
            {
                throw new Exception("Incompatible types");
            }

            if (argument is Float)
            {
                return new Float(value).Op(@operator, argument);
            }

            var other = argument as Integer;
            switch (@operator)
            {
                case Operator.Plus:
                    return new Integer(value + other.value);

                case Operator.Minus:
                    return new Integer(value - other.value);

                case Operator.Devide:
                    return new Float((float)value / (float)other.value);

                case Operator.DevideToInt:
                    return new Integer((int)((float)value / (float)other.value));

                case Operator.Multiply:
                    return new Integer(value * other.value);

                case Operator.Power:
                    return new Integer((int)Math.Pow(value, other.value));

                case Operator.Modulo:
                    return new Integer(value % other.value);

                case Operator.Equal:
                    return new Boolean(value == other.value);

                case Operator.NotEqual:
                    return new Boolean(value != other.value);

                case Operator.ShiftLeft:
                    return new Integer(value << other.value);

                case Operator.ShiftRight:
                    return new Integer(value >> other.value);

                case Operator.Lesser:
                    return new Boolean(value < other.value);

                case Operator.LesserOrEqual:
                    return new Boolean(value <= other.value);

                case Operator.Greater:
                    return new Boolean(value > other.value);

                case Operator.GreaterOrEqual:
                    return new Boolean(value >= other.value);
            }

            throw new RuntimeException($"Unknown operation for integer {@operator}");
        }

        public Float ToFloat()
        {
            return new Float(value);
        }

        public bool SameAs(Value other)
        {
            var otherInteger = other as Integer;

            return otherInteger != null
                && otherInteger.value == value;
        }
    }
}
