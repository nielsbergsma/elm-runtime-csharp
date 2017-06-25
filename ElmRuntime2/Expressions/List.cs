using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElmRuntime2.Exceptions;
using ElmRuntime2.Lexer;
using ElmRuntime2.Values;
using ElmRuntime2.Parser;

namespace ElmRuntime2.Expressions
{
    public class ListConstruct : Expression
    {
        private readonly Expression[] expressions;

        public ListConstruct(Expression[] expressions)
        {
            this.expressions = expressions;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            var values = new List<Value>();
            foreach(var expression in expressions)
            {
                var value = expression.Evaluate(arguments, scope) as Value;
                values.Add(value as Value);
            }

            return new List(values.ToArray());
        }
    }
}
