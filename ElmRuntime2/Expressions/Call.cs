using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElmRuntime2.Values;
using ElmRuntime2.Exceptions;
using ElmRuntime2.Parser;

namespace ElmRuntime2.Expressions
{
    public class Call : Expression
    {
        private readonly string name;
        private readonly Expression expression;
        private readonly List<Expression> arguments;
       
        public Call(string name, params Expression[] arguments)
        {
            this.name = name;
            this.arguments = arguments.ToList();
        }

        public Call(Expression expression, params Expression[] arguments)
        {
            this.expression = expression;
            this.arguments = arguments.ToList();
        }

        public string Name
        {
            get { return name; }
        }

        public Expression LastArgument
        {
            get { return arguments.Last(); }
        }

        public Expression PopArgument()
        {
            var expression = arguments.Last();
            arguments.RemoveAt(arguments.Count - 1);
            return expression;
        }

        public Expression Evaluate(Scope scope)
        {
            var expression = this.expression;
            if (name != null && !scope.TryGet(name, out expression))
            {
                throw new RuntimeException($"Unable to call {name}");
            }

            var arguments = this.arguments.ToList();
            while (arguments.Count > 0 && expression is Function)
            {
                var function = expression as Function;

                var functionArguments = new List<Expression>();
                if (function.NumberOfArguments > 0)
                {
                    functionArguments = arguments.Take(function.NumberOfArguments).ToList();
                    arguments = arguments.Skip(function.NumberOfArguments).ToList();
                }

                //eager evaluate arguments before calling
                for (var fa = 0; fa < functionArguments.Count; fa++)
                {
                    functionArguments[fa] = functionArguments[fa].Evaluate(scope);
                }

                expression = function.Evaluate(scope, functionArguments.ToArray());
            }

            return expression.Evaluate(scope);
        }

        public void PrependArgument(Expression argument)
        {
            arguments.Insert(0, argument);

            if (arguments.Count > 2)
            {
                throw new Exception("too many arguments");
            }
        }

        public void AppendArgument(Expression argument)
        {
            arguments.Add(argument);

            if (arguments.Count > 2)
            {
                throw new Exception("too many arguments");
            }
        }
    }
}
