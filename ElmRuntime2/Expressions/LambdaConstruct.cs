using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public class LambdaConstruct : Expression
    {
        private readonly Pattern[] arguments;
        private readonly Expression expression;

        public LambdaConstruct(Pattern[] arguments, Expression expression)
        {
            this.arguments = arguments;
            this.expression = expression;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            return new Lambda(scope.Unwrap(), this.arguments, this.expression).Evaluate(arguments, scope);
        }
    }
}
