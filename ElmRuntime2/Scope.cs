using ElmRuntime2.Expressions;
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
        private readonly Dictionary<string, Expression> values;

        public Scope()
            : this(null)
        {

        }

        public Scope(Scope parent)
        {
            this.parent = parent;
            this.values = new Dictionary<string, Expression>();
        }

        public void Set(string name, Expression value)
        {
            values[name] = value;
        }

        public bool TryGet(string name, out Expression value)
        {
            if (values.TryGetValue(name, out value))
            {
                return true;
            }
            else if (parent != null && parent.TryGet(name, out value))
            {
                return true;
            }
            else
            {
                value = default(Value);
                return false;
            }
        }

        public Dictionary<string, Expression> Unwrap()
        {
            return values;
        }
    }
}
