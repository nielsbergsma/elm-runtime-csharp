using ElmRuntime2.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser
{
    public class Module
    {
        private string name;
        private readonly List<ModuleExpose> exposing;
        private readonly List<ModuleImport> imports;
        private readonly Dictionary<string, Expression> expressions;

        public Module()
        {
            this.name = "default";
            this.imports = new List<ModuleImport>();
            this.exposing = new List<ModuleExpose>();
            this.expressions = new Dictionary<string, Expression>();
        }

        public static Module Parse(TokenStream stream, int position)
        {
            var module = new Module();

            while (!stream.IsAtEndOfStream(position))
            {
                //module header
                if (stream.IsAt(position, TokenType.Module))
                {
                    position = module.ParseNameAndExposing(stream, position);
                }
                else if (stream.IsAt(position, TokenType.Port, TokenType.Module))
                {
                    position = module.ParseNameAndExposing(stream, position + 1);
                }
                //import
                else if (stream.IsAt(position, TokenType.Import))
                {
                    position = module.ParseImport(stream, position);
                }
                //annotation
                else if (stream.IsAt(position, TokenType.Identifier, TokenType.Colon))
                {
                    position = stream.SkipUntilNextLine(position);
                }
                //expression
                else
                {
                    var start = position;
                    var end = stream.SkipUntilNextLine(start);

                    Console.WriteLine("--EXPRESSION START--");
                    for (; start < end; start++)
                    {
                        var token = stream.At(start);
                        Console.WriteLine($"Token {token.Type} {token.Content}");
                    }
                    Console.WriteLine("--EXPRESSION END--");

                    position = end;
                }
            }

            return module;
        }

        private int ParseNameAndExposing(TokenStream stream, int position)
        {
            if (stream.IsAt(position + 1, TokenType.Identifier))
            {
                name = stream.At(position + 1).Content;
                position += 2;
            }

            if (stream.IsAt(position, TokenType.Exposing))
            {
                var parsed = ModuleExposes.Parse(stream, position);
                exposing.AddRange(parsed.Value);
                position = parsed.Position;
            }

            return position;
        }

        private int ParseImport(TokenStream stream, int position)
        {
            var parsed = ModuleImports.Parse(stream, position);
            imports.AddRange(parsed.Value);
            return parsed.Position;
        }
    }
}
