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
        public static ParseResult<ModuleExpose[]> Parse(TokenStream stream, int position)
        {
            var parsed = Parser.ParseList(stream, position);
            var exposes = new List<ModuleExpose>();

            foreach(var expose in parsed.Value)
            {
                var name = expose.At(0).Content;

                //expose everything
                if (expose.IsAt(1, TokenType.LeftParen, TokenType.Range, TokenType.RightParen))
                {
                    exposes.Add(new ModuleUnresolvedExpose(name)); //, ".."?
                }
                //expose partial + constructors
                else if (expose.IsAt(1, TokenType.LeftParen))
                {
                    var constructors = Parser.ParseList(expose, 1);
                    foreach(var constructor in constructors.Value)
                    {
                        var identifier = constructor.At(0).Content;
                        exposes.Add(new ModuleUnresolvedExpose(name, identifier));
                    }
                }
                //expose 
                else
                {
                    exposes.Add(new ModuleUnresolvedExpose(name));
                }
            }

            var nextLinePosition = stream.SkipUntilNextLine(position);
            return new ParseResult<ModuleExpose[]>(exposes.ToArray(), nextLinePosition);
        }
    }

    public interface ModuleExpose
    {

    }

    public class ModuleUnresolvedExpose : ModuleExpose
    {
        private readonly string[] reference;

        public ModuleUnresolvedExpose(params string[] reference)
        {
            this.reference = reference;
        }

        public string[] Reference
        {
            get { return reference; }
        }
    }
}
