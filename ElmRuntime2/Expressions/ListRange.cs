using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElmRuntime2.Exceptions;
using ElmRuntime2.Values;
using ElmRuntime2.Parser;

namespace ElmRuntime2.Expressions
{
    public class ListRange : Expression
    {
        private readonly Expression from;
        private readonly Expression to;

        public ListRange(Expression from, Expression to)
        {
            this.from = from;
            this.to = to;
        }

        public Expression Evaluate(Value[] arguments, Scope scope)
        {
            var fromValue = from.Evaluate(arguments, scope);
            if (!(fromValue is Integer))
            {
                throw new RuntimeException("Range from is not an integer");
            }
            var fromInteger = (fromValue as Integer).Value;

            var toValue = to.Evaluate(arguments, scope);
            if (!(toValue is Integer))
            {
                throw new RuntimeException("Range from is not an integer");
            }
            var toInteger = (toValue as Integer).Value;

            var step = fromInteger <= toInteger ? 1 : -1;

            //TODO check if value is inclusive
            var values = new List<Value>();
            for (var v = fromInteger; v != toInteger; v += step) 
            {
                values.Add(new Integer(v));
            }

            return new List(values.ToArray());
        }

        public static ParseResult<ListRange> Parse(TokenStream stream, int position)
        {
            var parsed = ParserHelper.ParseList(stream, position);
            if (!parsed.Success)
            {
                return new ParseResult<ListRange>(false, default(ListRange), parsed.Position);
            }

            var from = ExpressionParser.Parse(parsed.Value[0], 0);
            var to = ExpressionParser.Parse(parsed.Value[0], from.Position + 1);

            if (from.Success && to.Success)
            {
                var range = new ListRange(from.Value, to.Value);
                return new ParseResult<ListRange>(true, range, parsed.Position);
            }
            else
            {
                throw new ParserException($"Unable to parse range expression near line { stream.LineOf(position) }");
            }
        }
    }
}
