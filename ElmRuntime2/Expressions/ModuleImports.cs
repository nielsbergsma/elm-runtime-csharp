using ElmRuntime2.Exceptions;
using ElmRuntime2.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser
{
    public interface ModuleImport
    {

    }

    public class ModuleUnresolvedImport : ModuleImport
    {
        private readonly string name;
        private readonly string alias;
        private readonly string[] selector;

        public ModuleUnresolvedImport(string name, string alias, params string[] selector)
        {
            this.name = name;
            this.alias = alias;
            this.selector = selector;
        }

        public string Name
        {
            get { return name; }
        }

        public string Alias
        {
            get { return alias; }
        }

        public string[] Selector
        {
            get { return selector; }
        }
    }
}