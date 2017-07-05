using ElmRuntime2.Native.Operators;
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

    public class Operator : IComparable<Operator>
    {
        private readonly string symbol;
        private int precedence;
        private OperatorAssociativity associativity;

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

        public OperatorAssociativity Associativity
        {
            get { return associativity; }
        }

        public int Precedence
        {
            get { return precedence; }
        }

        public void SetPrecedenceAndAssociativity(int precedence, OperatorAssociativity associativity)
        {
            this.precedence = precedence;
            this.associativity = associativity;
        }

        public int CompareTo(Operator other)
        {
            return Precedence - other.Precedence;
        }
    }
}