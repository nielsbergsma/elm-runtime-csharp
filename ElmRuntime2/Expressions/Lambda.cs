using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public class Lambda : Expression
    {
        private readonly Dictionary<string, Value> constants;
        private readonly FunctionArgument[] arguments;
        private readonly Expression expression;

        public Lambda(Dictionary<string, Value> constants, FunctionArgument[] arguments, Expression expression)
        {
            this.constants = constants;
            this.arguments = arguments;
            this.expression = expression;
        }

        public Expression Evaluate(Value[] arguments, Scope scope)
        {
            var lambdaScope = new Scope(scope);

            foreach(var constant in constants)
            {
                lambdaScope.SetValue(constant.Key, constant.Value);
            }

            for (var a = 0; a < arguments.Length && a < this.arguments.Length; a++)
            {
                this.arguments[a].SetScope(lambdaScope, arguments[a]);
            }

            if (arguments.Length < this.arguments.Length)
            {
                var newArguments = new List<FunctionArgument>();
                for (var a = arguments.Length; a < this.arguments.Length; a++)
                {
                    newArguments.Add(this.arguments[a]);
                }

                return new Lambda(lambdaScope.Unwrap(), newArguments.ToArray(), expression);
            }

            return expression.Evaluate(arguments, lambdaScope);
        }
    }
}
