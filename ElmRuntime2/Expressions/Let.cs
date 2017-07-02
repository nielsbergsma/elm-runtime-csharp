using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public class Let : Expression
    {
        private readonly Expression[] initialization;
        private readonly Expression result;

        public Let(Expression[] initialization, Expression result)
        {
            this.initialization = initialization;
            this.result = result;
        }

        public Expression Evaluate(Scope scope)
        {
            var letScope = new Scope(scope);

            foreach (var item in initialization)
            {
                if (item is Function)
                {
                    var function = item as Function;
                    letScope.Set(function.Name, function);
                }
                else
                {
                    item.Evaluate(letScope);
                }
            }

            return result.Evaluate(letScope);
        }
    }
}
