using ElmRuntime2.Parser;
using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2
{
    public class Scope
    {
        private readonly Scope parent;
        private readonly Dictionary<string, Value> values;

        public Scope()
            : this(null)
        {

        }

        public Scope(Scope parent)
        {
            this.parent = parent;
            this.values = new Dictionary<string, Value>();
        }

        public void SetValue(string name, Value value)
        {
            values[name] = value;
        }

        public bool TryGetValue(string name, out Value value)
        {
            if (values.TryGetValue(name, out value))
            {
                return true;
            }
            else if (parent != null && parent.TryGetValue(name, out value))
            {
                return true;
            }
            else
            {
                value = default(Value);
                return false;
            }
        }

        public Dictionary<string, Value> Unwrap()
        {
            return new Dictionary<string, Value>(this.values);
        }
    }
}
