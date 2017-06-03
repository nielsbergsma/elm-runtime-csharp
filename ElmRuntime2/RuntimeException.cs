using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2
{
    public class RuntimeException : Exception
    {
        public RuntimeException(string reason)
            : base(reason)
        {

        }
    }
}
