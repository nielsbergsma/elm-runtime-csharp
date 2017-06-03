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

        public static Module Parse(TokenStream stream)
        {
            var module = new Module();
            var position = 0;

            while (!stream.IsAtEndOfStream(position))
            {
                //module header
                if (stream.IsAt(position + 0, TokenType.Module))
                {
                    position = module.ParseNameAndExposing(stream, position);
                }
                else if (stream.IsAt(position + 0, TokenType.Port, TokenType.Module))
                {
                    position = module.ParseNameAndExposing(stream, position + 1);
                }
                //import
                else if (stream.IsAt(position + 0, TokenType.Import))
                {
                    position = module.ParseImport(stream, position);
                }
                //annotation
                else if (stream.IsAt(position + 0, TokenType.Identifier, TokenType.Colon))
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
            name = stream.At(position + 1).Content;

            if (!stream.IsAt(position + 2, TokenType.Exposing))
            {
                return position + 2;
            }

            //expose everything
            if (stream.IsAt(position + 3, TokenType.LeftParen, TokenType.Range, TokenType.RightParen))
            {
                exposing.Add(new ModuleUnresolvedExpose(".."));
                return position + 6;
            }
            //expose list
            else
            {
                var parsed = ModuleExposes.Parse(stream, position + 3);
                exposing.AddRange(parsed.Value);
                return parsed.Position;
            }
        }

        private int ParseImport(TokenStream stream, int position)
        {
            var parsed = ModuleImports.Parse(stream, position + 0);
            imports.AddRange(parsed.Value);
            return parsed.Position;
        }
    }
}
