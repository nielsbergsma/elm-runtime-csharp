using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public interface Pattern : Expression
    {
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

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            throw new NotImplementedException();
        }
    }

    public class UnderscorePattern : Pattern
    {
        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            return new Values.Boolean(true);
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

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            scope.Set(name, arguments[0]);

            return new Values.Boolean(true);
        }
    }

    public class LiteralPattern : Pattern
    {
        private readonly Value value;

        public LiteralPattern(Value value)
        {
            this.value = value;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            return new Values.Boolean(
                value.OperatorEquals(arguments[0])
            );
        }
    }

    public class ListPattern : Pattern
    {
        private readonly Pattern[] items;

        public ListPattern(Pattern[] items)
        {
            this.items = items;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            var list = arguments[0] as List;
            if (list == null || list.Values.Length != items.Length)
            {
                return new Values.Boolean(false);
            }

            for (var i = 0; i < items.Length; i++)
            {
                var argument = new Expression[] { list.Values[i] };
                var matches = items[i].Evaluate(argument, scope) as Values.Boolean;
                if (!matches.Value)
                {
                    return new Values.Boolean(false);
                }
            }

            return new Values.Boolean(true);
        }
    }

    public class TuplePattern : Pattern
    {
        private readonly Pattern[] items;

        public TuplePattern(Pattern[] items)
        {
            this.items = items;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            var tuple = arguments[0] as Values.Tuple;
            if (tuple == null || tuple.Values.Length != items.Length)
            {
                return new Values.Boolean(false);
            }

            for (var i = 0; i < items.Length; i++)
            {
                var argument = new Expression[] { tuple.Values[i] };
                var matches = items[i].Evaluate(argument, scope) as Values.Boolean;
                if (!matches.Value)
                {
                    return new Values.Boolean(false);
                }
            }

            return new Values.Boolean(true);
        }
    }

    public class RecordPattern : Pattern
    {
        private readonly Pattern[] fields;

        public RecordPattern(Pattern[] fields)
        {
            this.fields = fields;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            foreach (var field in fields)
            {
                var evaluationArguments = arguments;
                if (field is VariablePattern)
                {
                    var record = arguments[0] as Record;
                    var fieldName = (field as VariablePattern).Name;
                    var fieldValue = default(Expression);

                    record.TryGet(fieldName, out fieldValue);
                    evaluationArguments = new[] { fieldValue };
                }

                var matches = field.Evaluate(evaluationArguments, scope) as Values.Boolean;
                if (!matches.Value)
                {
                    return new Values.Boolean(false);
                }
            }

            return new Values.Boolean(true);
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

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            var list = arguments[0] as List;
            if (list.Values.Length == 0)
            {
                return new Values.Boolean(false);
            }

            var headResult = head.Evaluate(new Expression[] { list.Head() }, scope) as Values.Boolean;
            if (!headResult.Value)
            {
                return headResult;
            }

            return tail.Evaluate(new Expression[] { list.Tail() }, scope);
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

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            var union = arguments[0] as UnionConstructor;
            if (union.Constructor != constructor)
            {
                return new Values.Boolean(false);
            }
             
            for (var v = 0; v < values.Length; v++)
            {
                var argumentValue = default(Expression);
                if (!union.TryGet(v, out argumentValue))
                {
                    return new Values.Boolean(false);
                }

                var result = values[v].Evaluate(new[] { argumentValue }, scope) as Values.Boolean;
                if (!result.Value)
                {
                    return new Values.Boolean(false);
                }
            }

            return new Values.Boolean(true);
        }
    }
}
