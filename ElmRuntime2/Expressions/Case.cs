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

        public Expression Evaluate(Scope scope)
        {
            var caseScope = new Scope(scope);
            var subjectValue = subject.Evaluate(scope);

            foreach (var pattern in patterns)
            {
                var result = pattern.Evaluate(caseScope, subjectValue);
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
        private readonly Pattern condition;
        private readonly Expression expression;

        public CasePattern(Pattern condition, Expression expression)
        {
            this.condition = condition;
            this.expression = expression;
        }

        public Tuple<bool, Expression> Evaluate(Scope scope, Expression value)
        {
            var casePatternScope = new Scope(scope);

            var match = condition.Evaluate(casePatternScope, value);
            if (match)
            {
                var result = expression.Evaluate(casePatternScope);
                return System.Tuple.Create(true, result);
            }
            else
            {
                return System.Tuple.Create(false, default(Expression));
            }
        }
    }
}
