using ElmRuntime2.Expressions;
using ElmRuntime2.Lexer;
using ElmRuntime2.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Values
{
    public interface Value : Expression
    {
        bool OperatorEquals(Expression op2);
        bool OperatorLessThan(Expression op2);
    }
}
