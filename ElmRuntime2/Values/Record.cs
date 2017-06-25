using ElmRuntime2.Exceptions;
using ElmRuntime2.Expressions;
using ElmRuntime2.Lexer;
using ElmRuntime2.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Values
{
    public class Record : Value
    {
        private readonly Dictionary<string, Expression> fields;

        public Record()
            : this (new Dictionary<string, Expression>())
        {

        }

        private Record(Dictionary<string, Expression> fields)
        {
            this.fields = fields;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            return this;
        }

        public Record Set(string name, Value value)
        {
            return Set(new[] { new RecordFieldValue(name, value) });
        }

        public Record Set(params RecordFieldValue[] values)
        {
            var fields = new Dictionary<string, Expression>(this.fields);
            foreach (var value in values)
            {
                fields[value.Name] = value.Value;
            }
            return new Record(fields);
        }

        public bool TryGet(string name, out Expression value)
        {
            return fields.TryGetValue(name, out value);
        }

        public bool OperatorEquals(Expression op2)
        {
            var other = op2 as Record;
            if (other == null || other.fields.Count != fields.Count)
            {
                return false;
            }

            foreach (var otherField in other.fields)
            {
                if (!(otherField.Value is Value))
                {
                    return false;
                }

                var thisFieldValue = default(Expression);
                if (!fields.TryGetValue(otherField.Key, out thisFieldValue) || !(otherField .Value as Value).OperatorEquals(thisFieldValue))
                {
                    return false;
                }
            }

            return true;
        }

        public bool OperatorLesserThan(Expression op2)
        {
            return false;
        }
    }
}
