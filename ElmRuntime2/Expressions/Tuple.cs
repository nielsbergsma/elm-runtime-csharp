using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElmRuntime2.Values;
using ElmRuntime2.Exceptions;
using ElmRuntime2.Parser;
using ElmRuntime2.Lexer;

namespace ElmRuntime2.Expressions
{
    public class TupleConstruct : Expression
    {
        private readonly Expression[] expressions;

        public TupleConstruct(Expression[] expressions)
        {
            this.expressions = expressions;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            var values = new List<Value>();
            foreach(var expression in expressions)
            {
                var result = expression.Evaluate(arguments, scope);
                if (!(result is Value))
                {
                    throw new RuntimeException("Tuple expression must be evaluated to a value");
                }
                values.Add(result as Value);
            }

            return new Values.Tuple(values.ToArray());
        }        
    }
}
