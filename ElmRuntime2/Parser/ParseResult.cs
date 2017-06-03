using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser
{
    public class ParseResult<T>
    {
        private readonly bool success;
        private readonly T value;
        private readonly int position;

        public ParseResult(T value, int position)
            : this(true, value, position)
        {
        }

        public ParseResult(bool success, T value, int position)
        {
            this.success = success;
            this.value = value;
            this.position = position;
        }

        public bool Success
        {
            get { return success; }
        }

        public T Value
        {
            get { return value; }
        }

        public int Position
        {
            get { return position; }
        }
    }
}
