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
        private readonly Dictionary<string, Value> fields;

        public Record()
            : this (new Dictionary<string, Value>())
        {

        }

        private Record(Dictionary<string, Value> fields)
        {
            this.fields = fields;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            return this;
        }

        public Record Set(RecordFieldValue[] values)
        {
            var fields = new Dictionary<string, Value>(this.fields);
            foreach (var value in values)
            {
                fields[value.Name] = value.Value;
            }
            return new Record(fields);
        }

        public bool TryGet(string name, out Value value)
        {
            return fields.TryGetValue(name, out value);
        }

        public Value Op(Operator @operator)
        {
            throw new RuntimeException($"Unknown operation for tuple {@operator}");
        }

        public Value Op(Operator @operator, Value argument)
        {
            switch (@operator)
            {
                case Operator.Equal:
                    return new Boolean(SameAs(argument));

                case Operator.NotEqual:
                    return new Boolean(!SameAs(argument));
            }

            throw new RuntimeException($"Unknown operation for tulpe {@operator}");
        }

        public bool SameAs(Value other)
        {
            var otherRecord = other as Record;
            if (otherRecord == null || otherRecord.fields.Count != fields.Count)
            {
                return false;
            }

            foreach (var otherField in otherRecord.fields)
            {
                var thisFieldValue = default(Value);
                if (!fields.TryGetValue(otherField.Key, out thisFieldValue) || !otherField.Value.SameAs(thisFieldValue))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
