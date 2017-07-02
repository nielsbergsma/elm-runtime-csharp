using ElmRuntime2.Expressions;
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
            var tokens = new TokenStream(lexer);

            for (var t = 0; t < tokens.Length; t++)
            {
                Console.WriteLine($"Token [{t.ToString("000")}][{tokens.At(t).Type}] {tokens.At(t).Content}");
            }

            var module = ModuleParser.ParseModule(tokens, 0);
            var scope = new Scope();

            //var list = new List(
            //    new Integer(4),
            //    new Integer(2),
            //    new Integer(2),
            //    new Integer(2)
            //);

            //var tuple = new Values.Tuple(
            //    new Values.Integer(1),
            //    new Values.String("yes"),
            //    new Values.Integer(99)
            //);

            //var record = new Values.Record()
            //    .Set("foo", new Values.Integer(1))
            //    .Set("bar", new Values.String("you're welcome"))
            //    .Set("qux", new Values.Boolean(false));

            var union = new UnionConstructor("Some", new Integer(1), new Integer(-88));
            var tuple = new Values.Tuple(union, new Values.String("yes"));


            //new Integer(2)
            var result = module.Evaluate("main", tuple);
            Console.WriteLine();
            Console.WriteLine($"Evaluation result: {result} ({result.GetType().Name})");
         }        
    }
}
