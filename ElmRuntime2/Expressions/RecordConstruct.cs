using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElmRuntime2.Lexer;
using ElmRuntime2.Values;
using ElmRuntime2.Parser;

namespace ElmRuntime2.Expressions
{
    public class RecordConstruct : Expression
    {
        private readonly Dictionary<string, Expression> fieldExpressions;

        public RecordConstruct(Dictionary<string, Expression> fieldExpressions)
        {
            this.fieldExpressions = fieldExpressions;
        } 

        public Expression Evaluate(Scope scope)
        {
            var record = new Record();

            var fieldValues = new List<RecordFieldValue>();
            foreach (var fieldExpression in fieldExpressions)
            {
                var value = fieldExpression.Value.Evaluate(scope) as Value;
                var field = new RecordFieldValue(fieldExpression.Key, value);
                fieldValues.Add(field);
            }

            return record.Set(fieldValues.ToArray());
        }       
    }
}
