using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Lexer
{
    public enum Operator
    {
        Unknown = 0,

        ShiftLeft, // <<
        ShiftRight, // >>
        DevideToInt, // //
        Concat, // ++
        Prepend, // ::
        Equal, // ==
        NotEqual, // /=
        LesserOrEqual, // <=
        GreaterOrEqual, // >=
        And, // &&
        Or, // ||
        ContinueLeft, // |>
        ContinueRight, // <|
        Lesser, // <
        Greater, // >
        Devide, // /
        Multiply, // *
        Power, // ^
        Plus, // +
        Minus, // -
        Modulo, // %
    }

    public static class OperatorHelper
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
                    return Operator.ContinueLeft;
                case "<|":
                    return Operator.ContinueRight;
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
