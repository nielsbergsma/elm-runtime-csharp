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
        protected readonly string name;
        protected readonly Pattern[] arguments;
        protected readonly Expression expression;

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

        public int NumberOfArguments
        {
            get { return arguments.Length; }
        }

        public Pattern[] Arguments
        {
            get { return arguments; }
        }

        public Function Curry(Scope scope, Expression[] argumentValues)
        {
            if (argumentValues.Length == 0)
            {
                return this;
            }
            else
            {
                return new Closure(this, scope, argumentValues);
            }
        }

        public virtual Expression Evaluate(Scope scope)
        {
            return Evaluate(scope, new Expression[0]);
        }

        public virtual Expression Evaluate(Scope scope, Expression[] argumentValues)
        {
            if (argumentValues.Length < NumberOfArguments)
            {
                return Curry(scope, argumentValues);
            }

            //bring arguments into scope
            var functionScope = new Scope(scope);

            for (var a = 0; a < arguments.Length && a < argumentValues.Length; a++)
            {
                 arguments[a].Evaluate(functionScope, argumentValues[a]);
            }

            //evaluate
            return expression.Evaluate(functionScope);
        }        
    }
}
