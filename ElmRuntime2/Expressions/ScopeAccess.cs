using ElmRuntime2.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public class ScopeAccess : Expression
    {
        private readonly string name;

        public ScopeAccess(string name)
        {
            this.name = name;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            var result = default(Expression);
            if (!scope.TryGet(name, out result))
            {
                throw new RuntimeException($"Trying to access unknown scope expression {name}");
            }

            return result;
        }
    }
}
