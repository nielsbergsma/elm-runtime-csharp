using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public interface Expression
    {
        Expression Evaluate(Expression[] arguments, Scope scope);
    }
}
