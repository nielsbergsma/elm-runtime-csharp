using ElmRuntime2.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser.Values
{
    public class Record : Value
    {
        private readonly Dictionary<string, Value> fields;

        public Record(RecordFieldValue[] fieldValues)
        {
            fields = new Dictionary<string, Value>();
            foreach (var field in fieldValues)
            {
                fields[field.Name] = field.Value;
            }
        }

        private Record(Dictionary<string, Value> fields)
        {
            this.fields = fields;
        }

        public Record Update(RecordFieldValue[] updates)
        {
            var fields = new Dictionary<string, Value>(this.fields);
            foreach (var update in updates)
            {
                fields[update.Name] = update.Value;
            }
            return new Record(fields);
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
