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
    public class Float : Number
    {
        private readonly float value;

        public Float(float value)
        {
            this.value = value;
        }

        public float Value
        {
            get { return value; }
        }

        public Expression Evaluate(Value[] arguments, Scope scope)
        {
            return this;
        }

        public Value Op(Operator @operator)
        {
            if (@operator == Operator.Minus)
            {
                return new Float(-value);
            }

            throw new RuntimeException($"Unknown operation for float {@operator}");
        }

        public Value Op(Operator @operator, Value argument)
        {
            var other = default(Float);
            if (argument is Integer)
            {
                other = (argument as Integer).ToFloat();
            }
            else if (argument is Float)
            {
                other = argument as Float;
            }
            else
            {
                throw new Exception("Incompatible types");
            }

            switch (@operator)
            {
                case Operator.Plus:
                    return new Float(value + other.value);

                case Operator.Minus:
                    return new Float(value - other.value);

                case Operator.Devide:
                    return new Float((float)value / (float)other.value);

                case Operator.DevideToInt:
                    return new Integer((int)(value / other.value));

                case Operator.Multiply:
                    return new Float(value * other.value);

                case Operator.Power:
                    return new Float((float)Math.Pow(value, other.value));

                case Operator.Modulo:
                    return new Float(value % other.value);

                case Operator.Equal:
                    return new Boolean(value == other.value);

                case Operator.NotEqual:
                    return new Boolean(value != other.value);

                case Operator.Lesser:
                    return new Boolean(value < other.value);

                case Operator.LesserOrEqual:
                    return new Boolean(value <= other.value);

                case Operator.Greater:
                    return new Boolean(value > other.value);

                case Operator.GreaterOrEqual:
                    return new Boolean(value >= other.value);
            }

            throw new RuntimeException($"Unknown operation for float {@operator}");
        }

        public bool SameAs(Value other)
        {
            var otherFloat = other as Float;

            return otherFloat != null
                && otherFloat.value == value;
        }
    }
}
