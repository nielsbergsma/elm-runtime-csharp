using ElmRuntime2.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser
{
    public static class ModuleExposes
    {
        
    }

    public interface ModuleExpose
    {

    }

    public class ModuleUnresolvedExpose : ModuleExpose
    {
        private readonly string[] selector;

        public ModuleUnresolvedExpose(params string[] selector)
        {
            this.selector = selector;
        }

        public string[] Selector
        {
            get { return selector; }
        }
    }
}
