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
        private readonly Dictionary<string, Expression> constants;
        private readonly Pattern[] arguments;
        private readonly Expression expression;

        public Lambda(Dictionary<string, Expression> constants, Pattern[] arguments, Expression expression)
        {
            this.constants = constants;
            this.arguments = arguments;
            this.expression = expression;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            var lambdaScope = new Scope(scope);
            foreach(var constant in constants)
            {
                lambdaScope.Set(constant.Key, constant.Value);
            }

            for (var a = 0; a < arguments.Length && a < this.arguments.Length; a++)
            {
                this.arguments[a].Evaluate(new Expression[] { arguments[a] }, lambdaScope);
            }

            if (arguments.Length < this.arguments.Length)
            {
                var newArguments = new List<Pattern>();
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
