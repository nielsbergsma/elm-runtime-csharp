﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElmRuntime2.Parser.Values;

namespace ElmRuntime2.Parser
{
    public class Lambda : Expression
    {
        private readonly Dictionary<string, Value> constants;
        private readonly FunctionArgument[] arguments;
        private readonly Expression expression;

        public Lambda(Dictionary<string, Value> constants, FunctionArgument[] arguments, Expression expression)
        {
            this.constants = constants;
            this.arguments = arguments;
            this.expression = expression;
        }

        public Expression Evaluate(Value[] arguments, Scope scope)
        {
            var expressionScope = new Scope(scope);

            foreach(var constant in constants)
            {
                expressionScope.SetValue(constant.Key, constant.Value);
            }

            for (var a = 0; a < arguments.Length && a < this.arguments.Length; a++)
            {
                this.arguments[a].SetScope(expressionScope, arguments[a]);
            }

            if (arguments.Length < this.arguments.Length)
            {
                var newArguments = new List<FunctionArgument>();
                for (var a = arguments.Length; a < this.arguments.Length; a++)
                {
                    newArguments.Add(this.arguments[a]);
                }

                return new Lambda(expressionScope.Unwrap(), newArguments.ToArray(), expression);
            }

            return expression.Evaluate(arguments, expressionScope);
        }
    }
}
