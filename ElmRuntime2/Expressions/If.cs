using ElmRuntime2.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public class If : Expression
    {
        private readonly Expression condition;
        private readonly Expression then;
        private readonly Expression @else;

        public If(Expression condition, Expression then, Expression @else)
        {
            this.condition = condition;
            this.then = then;
            this.@else = @else;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            var result = condition.Evaluate(arguments, scope);
            if (!(result is Values.Boolean))
            {
                throw new RuntimeException("If condition did not result in boolean value");
            }

            if ((result as Values.Boolean).Value)
            {
                return then.Evaluate(arguments, scope);
            }
            else
            {
                return @else.Evaluate(arguments, scope);
            }
        }
    }
}
