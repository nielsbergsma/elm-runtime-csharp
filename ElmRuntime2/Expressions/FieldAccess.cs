using ElmRuntime2.Exceptions;
using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public class FieldAccess : Expression
    {
        private readonly string field;

        public FieldAccess(string field)
        {
            this.field = field;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            if (arguments.Length == 0)
            {
                return this;
            }

            if (arguments.Length > 1)
            {
                throw new RuntimeException("Too many arguments supplied");
            }

            var record = arguments[0].Evaluate(new Expression[0], scope) as Record;
            if (record == null)
            {
                throw new RuntimeException("FieldAccess argument is not a record");
            }

            var result = default(Expression);
            if (!record.TryGet(field, out result))
            {
                throw new RuntimeException($"field {field} does not exist in record");
            }

            return result;
        }
    }
}
