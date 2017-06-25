using ElmRuntime2.Lexer;
using ElmRuntime2.Parser;
using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = File.ReadAllText(@"c:\\elm\\helloworld1.elm");
            var lexer = ElmLexer.Lex(source);

            var position = 0;
            for (var token = lexer.Next(); token.HasValue; token = lexer.Next(), position++)
            {
                Console.WriteLine($"Token [{position} - {token.Value.Type}]{token.Value.Content}");
            }

            //add = \n -> ((\m n-> m + n) n)
            lexer.Reset();

            var tokens = new TokenStream(lexer);
            var module = Module.Parse(tokens, 0);
            var scope = new Scope();
            //var foo = new Record();
            //scope.Set("foo", foo);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var list = new List(
                new Integer(4)
                //new Integer(2),
                //new Integer(2),
                //new Integer(2),
            );

            //var tuple = new Values.Tuple(
            //    new Values.Integer(1),
            //    new Values.String("yes"),
            //    new Values.Integer(99),
            //);

            //var record = new Values.Record()
            //    .Set("foo", new Values.Integer(1))
            //    .Set("bar", new Values.String("you're welcome"))
            //    .Set("qux", new Values.Boolean(false));

            var union = new UnionConstructor("Some", new Integer(1), new Integer(-99));

            var tuple = new Values.Tuple(union, new Values.String("yes"));

            var result = module.Evaluate("main", new Value[] { tuple }, scope);

            stopwatch.Stop();
            Console.WriteLine($"Parsing + evaluating took={stopwatch.ElapsedMilliseconds}ms");
         }
    }
}
