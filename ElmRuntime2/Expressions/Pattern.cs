using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public interface Pattern
    {
        bool Evaluate(Scope scope, Expression value);
    }

    public class AliasPattern : Pattern
    {
        private readonly Pattern pattern;
        private readonly string alias;

        public AliasPattern(Pattern pattern, string alias)
        {
            this.pattern = pattern;
            this.alias = alias;
        }

        public bool Evaluate(Scope scope, Expression value)
        {
            scope.Set(alias, value);
            return pattern.Evaluate(scope, value);
        }
    }

    public class UnderscorePattern : Pattern
    {
        public bool Evaluate(Scope scope, Expression value)
        {
            return true;
        }
    }

    public class VariablePattern : Pattern
    {
        private readonly string name;

        public string Name
        {
            get { return name; }
        }

        public VariablePattern(string name)
        {
            this.name = name;
        }

        public bool Evaluate(Scope scope, Expression value)
        {
            scope.Set(name, value);
            return true;
        }
    }

    public class LiteralPattern : Pattern
    {
        private readonly Value value;

        public LiteralPattern(Value value)
        {
            this.value = value;
        }

        public bool Evaluate(Scope scope, Expression value)
        {
            return this.value.OperatorEquals(value);
        }
    }

    public class ListPattern : Pattern
    {
        private readonly Pattern[] items;

        public ListPattern(Pattern[] items)
        {
            this.items = items;
        }

        public bool Evaluate(Scope scope, Expression value)
        {

            var list = value as List;
            if (list == null || list.Values.Length != items.Length)
            {
                return false;
            }

            for (var i = 0; i < items.Length; i++)
            {
                var match = items[i].Evaluate(scope, list.Values[i]);
                if (!match)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class TuplePattern : Pattern
    {
        private readonly Pattern[] items;

        public TuplePattern(Pattern[] items)
        {
            this.items = items;
        }

        public bool Evaluate(Scope scope, Expression value)
        {
            var tuple = value as Values.Tuple;
            if (tuple == null || tuple.Values.Length != items.Length)
            {
                return false;
            }

            for (var i = 0; i < items.Length; i++)
            {
                var match = items[i].Evaluate(scope, tuple.Values[i]);
                if (!match)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class RecordPattern : Pattern
    {
        private readonly Pattern[] fields;

        public RecordPattern(Pattern[] fields)
        {
            this.fields = fields;
        }

        public bool Evaluate(Scope scope, Expression value)
        {
            foreach (var field in fields)
            {
                var fieldValue = default(Expression);
                if (field is VariablePattern)
                {
                    var record = value as Record;
                    var fieldName = (field as VariablePattern).Name;

                    record.TryGet(fieldName, out fieldValue);
                }

                var match = field.Evaluate(scope, fieldValue);
                if (!match)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class ListHeadTailPattern : Pattern
    {
        private readonly Pattern head;
        private readonly Pattern tail;

        public ListHeadTailPattern(Pattern head, Pattern tail)
        {
            this.head = head;
            this.tail = tail;
        }

        public bool Evaluate(Scope scope, Expression value)
        {
            var list = value as List;
            if (list.Values.Length == 0)
            {
                return false;
            }

            var match = head.Evaluate(scope, list.Head());
            if (!match)
            {
                return match;
            }

            return tail.Evaluate(scope, list.Tail());
        }
    }

    public class UnionConstructorPattern : Pattern
    {
        private readonly string constructor;
        private readonly Pattern[] values;

        public UnionConstructorPattern(string constructor, Pattern[] values)
        {
            this.constructor = constructor;
            this.values = values;
        }

        public bool Evaluate(Scope scope, Expression value)
        {
            var union = value as Union;
            if (union.Constructor != constructor)
            {
                return false;
            }

            for (var v = 0; v < values.Length; v++)
            {
                var argumentValue = default(Expression);
                if (!union.TryGet(v, out argumentValue))
                {
                    return false;
                }

                var result = values[v].Evaluate(scope, argumentValue);
                if (!result)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
