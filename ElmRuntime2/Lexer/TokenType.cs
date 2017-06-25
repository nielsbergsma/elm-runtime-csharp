using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Lexer
{
    public enum TokenType
    {
        /* errors */
        Unparsed = -1,
        Undetermined = 0,

        /* comment */
        Comment,

        /* symbols */
        LeftParen,
        RightParen,
        LeftBracket,
        RightBracket,
        LeftBrace,
        RightBrace,
        Pipe,
        Colon,
        Assign,
        Comma,
        Arrow,
        Backslash,
        Dot,
        Range,
        Underscore,

        /* identifier */
        Identifier,

        /* operators */
        OpInfix,
        OpPrefix,

        /* literals */
        String,
        Int,
        Float,
        Char,
        True,
        False,

        /* keywords */
        TypeDef,
        As,
        Alias,
        If,
        Then,
        Else,
        Of,
        Case,
        Infix,
        Infixl,
        Infixr,
        Let,
        In,
        Module,
        Exposing,
        Import,
        Port,

        /* lexer internal */
        MultiLineCommentStart,
        MultiLineCommentEnd,
        MultiLineStringBoundry,
        SingleLineCommentStart,
        StringBoundry
    }
}
