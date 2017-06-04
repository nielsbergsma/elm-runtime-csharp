using ElmRuntime2.Lexer;
using ElmRuntime2.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Exceptions
{
    public class RuntimeException : Exception
    {
        public RuntimeException(string reason)
            : base(reason)
        {

        }
    }
}
