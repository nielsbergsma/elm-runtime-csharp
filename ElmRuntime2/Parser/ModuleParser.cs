using ElmRuntime2.Exceptions;
using ElmRuntime2.Expressions;
using ElmRuntime2.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser
{
    public static class ModuleParser
    {
        public static Module ParseModule(TokenStream stream, int position)
        {
            var module = new Module();

            //preproccesing
            ParseOperators(stream, position, module);

            while (!stream.IsAtEndOfStream(position))
            {
                //module header
                if (stream.IsAt(position, TokenType.Module))
                {
                    position = ParseNameAndExposing(stream, position, module);
                }
                else if (stream.IsAt(position, TokenType.Port, TokenType.Module))
                {
                    position = ParseNameAndExposing(stream, position + 1, module);
                }
                //import
                else if (stream.IsAt(position, TokenType.Import))
                {
                    var parsed = ParseModuleImport(stream, position);
                    foreach(var import in parsed.Value)
                    {
                        module.Add(import);
                    }
                    position = parsed.Position;
                }
                //expression
                else
                {
                    var parsed = LineParser.ParseLine(stream, position, module);
                    if (parsed.Success)
                    {
                        if (parsed.Value is Function)
                        {
                            var function = parsed.Value as Function;
                            module.Add(function);
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

        private static int ParseNameAndExposing(TokenStream stream, int position, Module module)
        {
            module.SetName(stream.At(position + 1).Content);

            if (stream.IsAt(position + 2, TokenType.Exposing))
            {
                var parsed = ParseModuleExpose(stream, position + 2);
                foreach(var expose in parsed.Value)
                {
                    module.Add(expose);
                }

                position = parsed.Position;
            }

            return position;
        }

        private static void ParseOperators(TokenStream stream, int position, Module module)
        {
            //operators
            var start = position;
            for (; !stream.IsAtEndOfStream(position); position = stream.SkipToNextExpression(position))
            {
                if (stream.IsAt(position, TokenType.LeftParen) && stream.ContainsInExpression(position + 2, TokenType.RightParen))
                {
                    var rightParenPosition = stream.FindInExpression(position + 2, TokenType.RightParen);

                    var definition = stream.IsAt(1 + rightParenPosition.Value, TokenType.Colon);
                    if (!definition)
                    {
                        var symbol = "";
                        for (position++; position < rightParenPosition.Value; position++)
                        {
                            symbol += stream.At(position).Content;
                        }

                        module.Add(new Operator(symbol));
                    }
                }
            }

            //precedence & associativity
            position = start;
            for (; !stream.IsAtEndOfStream(position); position = stream.SkipToNextExpression(position))
            {
                if (stream.IsAnyAt(position, TokenType.Infix, TokenType.Infixl, TokenType.Infixr) && stream.IsAt(position + 1, TokenType.Int))
                {
                    var symbol = "";
                    var associativity = stream.At(position).Type;
                    var precedence = int.Parse(stream.At(position + 1).Content);
                    var end = stream.SkipToNextExpression(position);

                    for (position += 2; position < end; position++)
                    {
                        symbol += stream.At(position).Content;
                    }

                    var @operator = default(Operator);
                    if (!module.TryGetOperator(symbol, out @operator))
                    {
                        throw new ParserException($"Unable to find operator {symbol} to set precedence and associativity");
                    }

                    switch (associativity)
                    {
                        case TokenType.Infix:
                            @operator.SetPrecedenceAndAssociativity(precedence, OperatorAssociativity.NoAssociativity);
                            break;

                        case TokenType.Infixl:
                            @operator.SetPrecedenceAndAssociativity(precedence, OperatorAssociativity.Left);
                            break;

                        case TokenType.Infixr:
                            @operator.SetPrecedenceAndAssociativity(precedence, OperatorAssociativity.Right);
                            break;
                    }
                    position--;
                }
            }
        }

        private static ParseResult<ModuleExpose[]> ParseModuleExpose(TokenStream stream, int position)
        {
            var exposes = new List<ModuleExpose>();

            //expose everything
            if (stream.IsAt(position + 1, TokenType.LeftParen, TokenType.Range, TokenType.RightParen))
            {
                exposes.Add(new ModuleUnresolvedExpose());
                return new ParseResult<ModuleExpose[]>(exposes.ToArray(), position + 4);
            }

            //expose list
            var parsed = ParserHelper.ParseArray(stream, position + 1);
            foreach (var expose in parsed.Value)
            {
                var name = expose.At(0).Content;

                //operator
                if (expose.IsAt(0, TokenType.LeftParen) && expose.IsAt(expose.Length - 1, TokenType.RightParen))
                {
                    name = "";
                    for (var e = 1; e < expose.Length - 1; e++)
                    {
                        name += expose.At(e).Content;
                    }
                    exposes.Add(new ModuleUnresolvedExpose(name));
                }
                //expose everything
                else if (expose.IsAt(1, TokenType.LeftParen, TokenType.Range, TokenType.RightParen))
                {
                    exposes.Add(new ModuleUnresolvedExpose(name));
                }
                //expose partial + constructors
                else if (expose.IsAt(1, TokenType.LeftParen))
                {
                    var constructors = ParserHelper.ParseArray(expose, 1);
                    foreach (var constructor in constructors.Value)
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

            var nextExpressionStart = stream.SkipToNextExpression(position);
            return new ParseResult<ModuleExpose[]>(exposes.ToArray(), nextExpressionStart);
        }

        private static ParseResult<ModuleImport[]> ParseModuleImport(TokenStream stream, int position)
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
                    foreach (var type in types.Value)
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
                        foreach (var constructor in constructors.Value)
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
}
