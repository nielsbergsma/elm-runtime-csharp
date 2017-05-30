using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Lexer
{
    public interface Lexer
    {
        Maybe<Token> Next();
        void Reset();
    }
}
