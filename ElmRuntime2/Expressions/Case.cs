using ElmRuntime2.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Expressions
{
    public class Case : Expression
    {
        private readonly Expression subject;
        private readonly CasePattern[] patterns;

        public Case(Expression subject, CasePattern[] patterns)
        {
            this.subject = subject;
            this.patterns = patterns;
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            var caseScope = new Scope(scope);
            var patternArguments = new Expression[] {
                subject.Evaluate(arguments, scope)
            };

            foreach (var pattern in patterns)
            {
                var result = pattern.Evaluate(patternArguments, caseScope);
                if (result.Item1)
                {
                    return result.Item2;
                }
            }

            throw new RuntimeException("Unexpected behaviour, didn't match any of the case expression");
        }
    }

    public class CasePattern
    {
        private readonly Expression condition;
        private readonly Expression expression;

        public CasePattern(Expression condition, Expression expression)
        {
            this.condition = condition;
            this.expression = expression;
        }

        public Tuple<bool, Expression> Evaluate(Expression[] arguments, Scope scope)
        {
            var casePatternScope = new Scope(scope);
            var matches = condition.Evaluate(arguments, casePatternScope);

            if (matches is Values.Boolean && (matches as Values.Boolean).Value)
            {
                var result = expression.Evaluate(arguments, casePatternScope);
                return Tuple.Create(true, result);
            }
            else
            {
                return Tuple.Create(false, default(Expression));
            }
        }
    }
}
