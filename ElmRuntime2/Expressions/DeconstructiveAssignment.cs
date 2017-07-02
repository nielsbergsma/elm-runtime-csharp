using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public class DeconstructiveAssignment : Expression
    {
        private readonly Pattern pattern;
        private readonly Expression expression;

        public DeconstructiveAssignment(Pattern pattern, Expression expression)
        {
            this.pattern = pattern;
            this.expression = expression;
        }

        public Expression Evaluate(Scope scope)
        {
            var value = expression.Evaluate(scope);
            pattern.Evaluate(scope, value);
            return value;
        }
    }
}
