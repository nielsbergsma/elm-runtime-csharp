using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Lexer
{
    public static class ElmLexer
    {
        public static Lexer Lex(string input)
        {
            //replace \r and tabs by spaces
            input = input.Replace("\r", "").Replace("\t", "    ");

            var lexer = new LineLexer(input) as Lexer;

            //multi line lexers
            lexer = new MultiLineCommentLexer(lexer);
            lexer = new MultiLineStringLexer(lexer);

            //enclosed literals
            lexer = new StringLexer(lexer);
            lexer = new CharLexer(lexer);

            //rest
            lexer = new SingleLineCommentLexer(lexer);
            lexer = new NumberLexer(lexer);
            lexer = new IdentifierLexer(lexer);
            lexer = new KeywordLexer(lexer);
            lexer = new SymbolLexer(lexer);

            //clean up
            lexer = new WhitespaceIgnoreLexer(lexer);
            lexer = new CommentIgnoreLexer(lexer);

            return lexer;
        }
    }
}
