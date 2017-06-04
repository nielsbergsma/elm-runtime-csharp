using ElmRuntime2.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser.Values
{
    public interface Value : Expression
    {
        Value Op(Operator @operator);
        Value Op(Operator @operator, Value argument);
        bool SameAs(Value other);
    }
}
