using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElmRuntime2.Values;
using ElmRuntime2.Exceptions;

namespace ElmRuntime2.Expressions
{
    public class Invocation : Expression
    {
        private readonly string name;
        private readonly Expression[] arguments;

        public Invocation(string name, Expression[] arguments)
        {
            this.name = name;
            this.arguments = arguments;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            var expression = default(Expression);
            if (scope.TryGet(name, out expression))
            {
                return expression.Evaluate(this.arguments, scope);
            }
            throw new RuntimeException($"Unable to invoke {name}");
        }
    }
}
