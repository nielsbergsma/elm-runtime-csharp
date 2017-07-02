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

            this.operators = new Dictionary<string, Operator>(Native.Operators.Operators.Core); //temp
            Add(new Native.Operators.Plus());
            Add(new Native.Operators.Minus());
            Add(new Native.Operators.Negate());
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public void Add(Operator @operator)
        {
            operators[@operator.Symbol] = @operator;
        }

        public void Add(ModuleExpose expose)
        {
            exposing.Add(expose);
        }

        public void Add(ModuleImport import)
        {
            imports.Add(import);
        }

        public void Add(Function function)
        {
            expressions[function.Name] = function;
        }

        public IEnumerable<Operator> Operators
        {
            get { return operators.Values; }
        }

        public bool TryGetOperator(string symbol, out Operator @operator)
        {
            return operators.TryGetValue(symbol, out @operator);
        }

        public Expression Evaluate(string name, params Expression[] arguments)
        {
            var scope = new Scope();

            //bring imports and expressions into scope
            foreach(var expression in expressions)
            {
                scope.Set(expression.Key, expression.Value);
            }

            var call = new Call(name, arguments);
            return call.Evaluate(scope);
        }        
    }
}
