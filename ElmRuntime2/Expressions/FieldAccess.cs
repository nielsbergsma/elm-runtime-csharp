using ElmRuntime2.Exceptions;
using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public class FieldAccess : Function
    {
        private readonly string field;

        public FieldAccess(string field)
            : base(field + "_get", new[] { new UnderscorePattern() }, null)
        {
            this.field = field;
        }

        public override Expression Evaluate(Scope scope, Expression[] argumentValues)
        {
            var record = argumentValues[0].Evaluate(scope) as Record;
            var result = default(Expression);

            if (!record.TryGet(field, out result))
            {
                throw new RuntimeException($"Record doesn't have field {field}");
            }

            return result;
        }
    }
}
