using ElmRuntime2.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser
{
    public static class ModuleImports
    {
        public static ParseResult<ModuleImport[]> Parse(TokenStream stream, int position)
        {
            var imports = new List<ModuleImport>();
            if (!stream.IsAt(position + 1, TokenType.Identifier))
            {
                return new ParseResult<ModuleImport[]>(imports.ToArray(), position + 1);
            }

            var name = "";
            for (var index = position + 1; index < stream.Length && stream.IsAnyAt(index, TokenType.Identifier, TokenType.Dot); index++)
            {
                name += stream.At(index).Content;
            }

            var alias = name;
            var aliasStart = stream.FindInExpression(position + 1, TokenType.As);
            if (aliasStart.HasValue)
            {
                alias = stream.At(aliasStart.Value + 1).Content;
            }

            if (stream.IsAt(position + 2, TokenType.Exposing))
            {
                if (stream.IsAt(position + 3, TokenType.LeftParen, TokenType.Range, TokenType.RightParen))
                {
                    imports.Add(new ModuleUnresolvedImport(name, alias));
                }
                else
                {
                    var types = ParserHelper.ParseList(stream, position + 3);
                    foreach(var type in types.Value)
                    {
                        var typeIdentifier = type.At(0).Content;
                        var constructors = ParserHelper.ParseList(type, 1);

                        if (constructors.Value.Length == 0)
                        {
                            imports.Add(new ModuleUnresolvedImport(name, alias, typeIdentifier));
                        }

                        foreach(var constructor in constructors.Value)
                        {
                            var constructorIdentifier = constructor.At(0).Content;
                            if (constructorIdentifier == "..")
                            {
                                imports.Add(new ModuleUnresolvedImport(name, alias, typeIdentifier));
                            }
                            else
                            {
                                imports.Add(new ModuleUnresolvedImport(name, alias, typeIdentifier, constructorIdentifier));
                            }
                        }
                    }
                }
            }
            else
            {
                imports.Add(new ModuleUnresolvedImport(name, alias));
            }

            var nextExpressionStart = stream.SkipToNextExpression(position);
            return new ParseResult<ModuleImport[]>(imports.ToArray(), nextExpressionStart); 
        }
    }

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