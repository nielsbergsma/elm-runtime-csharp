using ElmRuntime2.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Native.Operators
{
    public static class Operators
    {
        public static Dictionary<string, Operator> Core = new Dictionary<string, Operator>
        {
            { ">>", new Operator(">>", 9, OperatorAssociativity.Left) },
            { "<<", new Operator("<<", 9, OperatorAssociativity.Right) },
            { "^", new Operator("^", 8, OperatorAssociativity.Right) },
            { "*", new Operator("*", 7, OperatorAssociativity.Left) },
            { "%", new Operator("%", 7, OperatorAssociativity.Left) },
            { "/", new Operator("/", 7, OperatorAssociativity.Left) },
            { "//", new Operator("//", 7, OperatorAssociativity.Left) },
            { "+", new Operator("+", 6, OperatorAssociativity.Left) },
            { "-", new Operator("-", 6, OperatorAssociativity.Left) },
            { "++", new Operator("++", 5, OperatorAssociativity.Right) },
            { "::", new Operator("::", 5, OperatorAssociativity.Right) },

            { "==", new Operator("==", 4, OperatorAssociativity.NoAssociativity) },
            { "/=", new Operator("/=", 4, OperatorAssociativity.NoAssociativity) },
            { "<", new Operator("<", 4, OperatorAssociativity.NoAssociativity) },
            { ">", new Operator(">", 4, OperatorAssociativity.NoAssociativity) },
            { "<=", new Operator("<=", 4, OperatorAssociativity.NoAssociativity) },
            { ">=", new Operator(">=", 4, OperatorAssociativity.NoAssociativity) },

            { "&&", new Operator("&&", 3, OperatorAssociativity.Right) },
            { "||", new Operator("||", 2, OperatorAssociativity.Right) },
            { "<|", new Operator("<|", 0, OperatorAssociativity.Right) },
            { "|>", new Operator("|>", 0, OperatorAssociativity.Left) },
        };
    }
}
