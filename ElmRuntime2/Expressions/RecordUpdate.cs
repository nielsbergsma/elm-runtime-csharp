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
    public class RecordUpdate : Expression
    {
        private readonly string name;
        private readonly Dictionary<string, Expression> fieldExpressions;

        public RecordUpdate(string name, Dictionary<string, Expression> fieldExpressions)
        {
            this.name = name;
            this.fieldExpressions = fieldExpressions;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            var recordValue = default(Expression);
            if (!scope.TryGet(name, out recordValue) || !(recordValue is Record))
            {
                throw new RuntimeException($"Variable {name} is not a record");
            }
            var record = recordValue as Record;

            var fieldValues = new List<RecordFieldValue>();
            foreach (var fieldExpression in fieldExpressions)
            {
                var value = fieldExpression.Value.Evaluate(arguments, scope) as Value;
                var field = new RecordFieldValue(fieldExpression.Key, value);
                fieldValues.Add(field);
            }

            return record.Set(fieldValues.ToArray());
        }
    }
}
