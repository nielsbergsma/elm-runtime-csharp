using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public class Group : Expression
    {
        private readonly Expression expression;

        public Group(Expression expression)
        {
            this.expression = expression;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            return expression.Evaluate(arguments, scope);
        }
    }
}
