using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Values
{
    public class RecordFieldValue
    {
        private readonly string name;
        private readonly Value value;

        public RecordFieldValue(string name, Value value)
        {
            this.name = name;
            this.value = value;
        }

        public string Name
        {
            get { return name; }
        }

        public Value Value
        {
            get { return value; }
        }
    }
}
