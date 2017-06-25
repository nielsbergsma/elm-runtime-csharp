using ElmRuntime2.Exceptions;
using ElmRuntime2.Expressions;
using ElmRuntime2.Lexer;
using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser
{
    public enum OperatorAssociativity
    {
        Left,
        NoAssociativity,
        Right
    }

    public class Operator : Expression, IComparable<Operator>
    {
        private readonly string symbol;
        private int precedence;
        private OperatorAssociativity associativity;
        private Function evaluator;

        public Operator(string symbol)
            : this(symbol, 9, OperatorAssociativity.Left)
        {
        }

        public Operator(string symbol, int precedence, OperatorAssociativity associativity)
        {
            this.symbol = symbol;
            this.precedence = precedence;
            this.associativity = associativity;
        }

        public string Symbol
        {
            get { return symbol; }
        }

        public void SetPrecedenceAndAssociativity(int precedence, OperatorAssociativity associativity)
        {
            this.precedence = precedence;
            this.associativity = associativity;
        }

        public void SetEvaluator(Function evaluator)
        {
            this.evaluator = evaluator;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            return evaluator.Evaluate(arguments, scope);
        }

        public int Precedence
        {
            get { return precedence; }
        }

        public int CompareTo(Operator other)
        {
            return Precedence - other.Precedence;
        }
    }

    public static class Core
    {
        public static Dictionary<string, Operator> Operators = new Dictionary<string, Operator>
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