using ElmRuntime2.Parser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser
{
    public interface Expression
    {
        Expression Evaluate(Value[] arguments, Scope scope);
    }
}
