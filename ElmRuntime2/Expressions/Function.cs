using ElmRuntime2.Exceptions;
using ElmRuntime2.Lexer;
using ElmRuntime2.Parser;
using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public class Function : Expression
    {
        private readonly string name;
        private readonly Pattern[] arguments;
        private readonly Expression expression;

        public Function(string name, Pattern[] arguments, Expression expression)
        {
            this.name = name;
            this.arguments = arguments;
            this.expression = expression;
        }

        public string Name
        {
            get { return name; }
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            var expressionScope = new Scope(scope);

            //bring arguments into scope
            for (var a = 0; a < arguments.Length && a < this.arguments.Length; a++)
            {
                this.arguments[a].Evaluate(new Expression[] { arguments[a] }, expressionScope);
            }

            //curry function
            if (arguments.Length < this.arguments.Length)
            {
                var newArguments = new List<Pattern>();
                for (var a = arguments.Length; a < this.arguments.Length; a++)
                {
                    newArguments.Add(this.arguments[a]);
                }

                return new Lambda(expressionScope.Unwrap(), newArguments.ToArray(), expression);
            }

            //evaluate
            return expression.Evaluate(arguments, expressionScope);
        }        
    }
}
