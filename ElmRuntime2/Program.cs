using ElmRuntime2.Lexer;
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
            var input = File.ReadAllText(@"c:\\elm\\helloworld1.elm");

            var valid = ElmLexer.Validate(input);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var lexer = ElmLexer.Lex(input);

            for (var token = lexer.Next(); token.HasValue; token = lexer.Next())
            {
                Console.WriteLine($"Token [{token.Value.Type}:{token.Value.Line+1}:{token.Value.Column+1}]{token.Value.Content}");

                if (token.Value.Type == TokenType.Unknown || token.Value.Type == TokenType.Unparsed)
                {
                    Console.WriteLine($"^^^^^ <-- look out, type={token.Value.Type}");
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"Lexing valid={valid}, took={stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
