using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser
{
    public enum Operator
    {
        Unknown = 0,

        /* << */ ShiftLeft,
        /* >> */ ShiftRight,
        /* // */ DevideToInt,
        /* ++ */ Concat,
        /* :: */ Prepend,
        /* == */ Equal,
        /* /= */ NotEqual,
        /* <= */ LesserOrEqual,
        /* >= */ GreaterOrEqual,
        /* && */ And,
        /* || */ Or,
        /* |> */ TransformLeft,
        /* <| */ TransformRight,
        /* <  */ Lesser,
        /* >  */ Greater,
        /* /  */ Devide,
        /* *  */ Multiply,
        /* ^  */ Power,
        /* +  */ Plus,
        /* -  */ Minus,
        /* %  */ Modulo,
    }

    public static class OperatorParser
    {
        public static Operator Parse(string value)
        {
            switch (value)
            {
                case "<<":
                    return Operator.ShiftLeft;
                case ">>":
                    return Operator.ShiftRight;
                case "//":
                    return Operator.DevideToInt;
                case "++":
                    return Operator.Concat;
                case "::":
                    return Operator.Prepend;
                case "==":
                    return Operator.Equal;
                case "/=":
                    return Operator.NotEqual;
                case "<=":
                    return Operator.LesserOrEqual;
                case ">=":
                    return Operator.GreaterOrEqual;
                case "&&":
                    return Operator.And;
                case "||":
                    return Operator.Or;
                case "|>":
                    return Operator.TransformLeft;
                case "<|":
                    return Operator.TransformRight;
                case "<":
                    return Operator.Lesser;
                case ">":
                    return Operator.Greater;
                case "/":
                    return Operator.Devide;
                case "*":
                    return Operator.Multiply;
                case "^":
                    return Operator.Power;
                case "+":
                    return Operator.Plus;
                case "-":
                    return Operator.Minus;
                case "%":
                    return Operator.Modulo;
                default:
                    return Operator.Unknown;
            }
        }
    }
}
