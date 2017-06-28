using ElmRuntime2.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public class OperatorInvocation : Expression
    {
        private readonly Operator @operator;
        private Expression lhs;
        private Expression rhs;

        public OperatorInvocation(Operator @operator)
        {
            this.@operator = @operator;
        }

        public Operator Operator
        {
            get { return @operator; }
        }

        public void SetLHS(Expression lhs)
        {
            this.lhs = lhs;
        }

        public void SetRHS(Expression rhs)
        {
            this.rhs = rhs;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            return @operator.Evaluate(new[] { lhs, rhs }, scope);
        }
    }
}
