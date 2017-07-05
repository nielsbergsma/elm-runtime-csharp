using ElmRuntime2.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public class BinaryOperationCall : Call
    {
        private readonly int precedence;
        private readonly OperatorAssociativity associativity;

        public BinaryOperationCall(Operator @operator, params Expression[] arguments)
            : this(@operator.Symbol, @operator.Precedence, @operator.Associativity, arguments)
        {
            //forward constructor
        }

        public BinaryOperationCall(Operator @operator, int precedence, params Expression[] arguments)
            : this(@operator.Symbol, precedence, @operator.Associativity, arguments)
        {
            //forward constructor
        }

        public BinaryOperationCall(string name, int precedence, OperatorAssociativity associativity, params Expression[] arguments)
            : base(name, arguments)
        {
            this.precedence = precedence;
            this.associativity = associativity;
        }

        public int Precedence
        {
            get { return precedence; }
        }

        public OperatorAssociativity Associativity
        {
            get { return associativity; }
        }
    }
}
