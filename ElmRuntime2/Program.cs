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
            lexer.Reset();

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var tokens = new TokenStream(lexer);
            var module = Module.Parse(tokens, 0);

            var scope = new Scope();
            var foo = new Record();
            scope.Set("foo", foo);

            var result = module.Evaluate("main", new Value[0], scope);

            stopwatch.Stop();
            Console.WriteLine($"Parsing + evaluating took={stopwatch.ElapsedMilliseconds}ms, result={result}");
        }
    }
}
