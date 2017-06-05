using ElmRuntime2.Exceptions;
using ElmRuntime2.Expressions;
using ElmRuntime2.Lexer;
using ElmRuntime2.Values;
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
            this.name = "Default";
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
                //expression
                else
                {
                    var parsed = LineParser.Parse(stream, position);
                    if (parsed.Success)
                    {
                        if (parsed.Value is Function)
                        {
                            var function = parsed.Value as Function;
                            module.expressions[function.Name] = function;
                        }
                        else
                        {
                            throw new ParserException($"Not supported expression {parsed.Value} at module level");
                        }
                    }

                    position = parsed.Position;
                }
            }

            return module;
        }

        public Expression Evaluate(string name)
        {
            return Evaluate(name, new Value[0], new Scope());
        }

        public Expression Evaluate(string name, Value[] arguments, Scope scope)
        {
            //bring imports and expressions into scope
            foreach(var expression in expressions)
            {
                scope.Set(expression.Key, expression.Value);
            }

            return expressions[name].Evaluate(arguments, scope);
        }

        private int ParseNameAndExposing(TokenStream stream, int position)
        {
            name = "";
            for (position++; position < stream.Length && stream.IsAnyAt(position, TokenType.Identifier, TokenType.Dot); position++)
            {
                name += stream.At(position).Content;
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
