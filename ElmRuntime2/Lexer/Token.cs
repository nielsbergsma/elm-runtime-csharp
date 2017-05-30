﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Lexer
{
    public class Token
    {
        private readonly int line;
        private readonly int column;
        private readonly TokenType type;
        private readonly string content;

        public Token(int line, int column, TokenType type, string content)
        {
            this.line = line;
            this.column = column;
            this.type = type;
            this.content = content;
        }

        public int Line
        {
            get { return line; }
        }

        public int Column
        {
            get { return column; }
        }

        public TokenType Type
        {
            get { return type; }
        }

        public string Content
        {
            get { return content; }
        }
        
        public bool Is(TokenType type)
        {
            return this.type == type;
        }

        public TokenSplitResult Split(int index, string seperator)
        {
            var before = Maybe<Token>.None();
            var after = Maybe<Token>.None();

            if (index > 0)
            {
                var token = new Token(line, column, type, content.Substring(0, index));
                before = Maybe<Token>.Some(token);
            }

            var offset = index + seperator.Length;
            if (offset <= content.Length)
            {
                var token = new Token(line, column + offset, type, content.Substring(offset));
                after = Maybe<Token>.Some(token);
            }

            return new TokenSplitResult(before, after, seperator);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Token;

            return other.line == line
                && other.column == column
                && other.type == type
                && other.content == content;
        }

        public override int GetHashCode()
        {
            var hash = line.GetHashCode();
            hash = (hash * 397) ^ column.GetHashCode();
            hash = (hash * 397) ^ type.GetHashCode();
            hash = (hash * 397) ^ content.GetHashCode();
            return hash;
        }
    }
}
