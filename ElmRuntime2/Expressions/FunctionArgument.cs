using ElmRuntime2.Exceptions;
using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public interface FunctionArgument
    {
        void SetScope(Scope scope, Value value);
    }

    public class FunctionNamedArgument : FunctionArgument
    {
        private readonly string name;

        public FunctionNamedArgument(string name)
        {
            this.name = name;
        }

        public void SetScope(Scope scope, Value value)
        {
            scope.SetValue(name, value);
        }
    }

    public class FunctionDeconstructiveRecordArgument : FunctionArgument
    {
        private readonly string[] names;

        public FunctionDeconstructiveRecordArgument(string[] names)
        {
            this.names = names;
        }

        public void SetScope(Scope scope, Value value)
        {
            if (!(value is Record))
            {
                throw new RuntimeException($"Expected record value to deconstruct, got {value.GetType()}");
            }

            var record = value as Record;
            foreach(var name in names)
            {
                if (record.TryGet(name, out value))
                {
                    scope.SetValue(name, value);
                }
            }
        }
    }

    public class FunctionDeconstructiveTupleArgument : FunctionArgument
    {
        private readonly string[] names;

        public FunctionDeconstructiveTupleArgument(string[] names)
        {
            this.names = names;
        }

        public void SetScope(Scope scope, Value value)
        {
            if (!(value is Values.Tuple))
            {
                throw new RuntimeException($"Expected tuple value to deconstruct, got {value.GetType()}");
            }

            var tuple = value as Values.Tuple;
            for(var item = 0; item < names.Length; item++)
            {
                if (tuple.TryGet(item, out value))
                {
                    scope.SetValue(names[item], value);
                }
            }
        }
    }
}
