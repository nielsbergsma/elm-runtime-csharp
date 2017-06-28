using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Values
{
    public interface Number : Value
    {
        int AsInt();
        float AsFloat();
    }
}
