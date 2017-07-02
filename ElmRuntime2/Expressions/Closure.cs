using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public class Closure : Function
    {
        private readonly Scope scope;
        private readonly Expression[] argumentValues;

        public Closure(Function function, Scope scope, Expression[] argumentValues)
            : base($"{function.Name}_closure", function.Arguments.Skip(argumentValues.Length).ToArray(), function)
        {
            this.scope = scope;
            this.argumentValues = argumentValues;
        }

        public override Expression Evaluate(Scope scope, Expression[] argumentValues)
        {
            var function = expression as Function;
            var closureArgumentValues = this.argumentValues.Union(argumentValues);

            return function.Evaluate(this.scope, closureArgumentValues.ToArray());
        }
    }
}
