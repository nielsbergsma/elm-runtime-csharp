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
        private readonly Dictionary<string, Operator> operators;

        public Module()
        {
            this.name = "Unnamed";
            this.imports = new List<ModuleImport>();
            this.exposing = new List<ModuleExpose>();
            this.expressions = new Dictionary<string, Expression>();
            this.operators = new Dictionary<string, Operator>(Core.Operators); //temp
        }

        public static Module Parse(TokenStream stream, int position)
        {
            var module = new Module();

            //preproccesing
            module.ParseOperators(stream, position);

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
                    var parsed = LineParser.ParseLine(stream, position, module);
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

        public IEnumerable<Operator> Operators
        {
            get { return operators.Values; }
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
            name = stream.At(position + 1).Content;

            if (stream.IsAt(position + 2, TokenType.Exposing))
            {
                var parsed = ModuleExposes.ParseModule(stream, position + 2);
                exposing.AddRange(parsed.Value);
                position = parsed.Position;
            }

            return position;
        }

        private int ParseImport(TokenStream stream, int position)
        {
            var parsed = ModuleImports.ParseImport(stream, position);
            imports.AddRange(parsed.Value);
            return parsed.Position;
        }

        private void ParseOperators(TokenStream stream, int position)
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

                        operators[symbol] = new Operator(symbol);
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
                    if (!operators.TryGetValue(symbol, out @operator))
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
    }
}
