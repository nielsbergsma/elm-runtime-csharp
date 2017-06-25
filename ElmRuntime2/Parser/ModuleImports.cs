using ElmRuntime2.Exceptions;
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
        public static ParseResult<ModuleImport[]> ParseImport(TokenStream stream, int position)
        {
            var imports = new List<ModuleImport>();

            var name = stream.At(position + 1).Content;
            position++;

            //alias
            var alias = name;
            var aliasStart = stream.FindInExpression(position - 1, TokenType.As);
            if (aliasStart.HasValue && stream.IsAt(aliasStart.Value, TokenType.As, TokenType.Identifier))
            {
                alias = stream.At(aliasStart.Value + 1).Content;
            }

            //exposing
            if (stream.IsAt(position, TokenType.Exposing))
            {
                //everything
                if (stream.IsAt(position + 1, TokenType.LeftParen, TokenType.Range, TokenType.RightParen))
                {
                    imports.Add(new ModuleUnresolvedImport(name, alias));
                }
                //list
                else
                {
                    var types = ParserHelper.ParseArray(stream, position + 1);
                    foreach(var type in types.Value)
                    {
                        var typeIdentifier = type.At(0).Content;
                        var constructors = ParserHelper.ParseArray(type, 1);

                        //operator
                        if (type.IsAt(0, TokenType.LeftParen) && type.IsAt(type.Length - 1, TokenType.RightParen))
                        {
                            typeIdentifier = "";
                            for (var ti = 1; ti < type.Length - 1; ti++)
                            {
                                typeIdentifier += type.At(ti).Content;
                            }
                            imports.Add(new ModuleUnresolvedImport(name, alias, typeIdentifier));
                        }
                        //named expression / type
                        else if (constructors.Value.Length == 0)
                        {
                            imports.Add(new ModuleUnresolvedImport(name, alias, typeIdentifier));
                        }

                        //union constructors
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