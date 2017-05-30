﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ElmRuntime2.Lexer
{
    public class NumberLexer : Lexer
    {
        private readonly Lexer source;
        private readonly Stack<Token> head;
        private readonly Regex number;

        public NumberLexer(Lexer source)
        {
            this.source = source;
            this.head = new Stack<Token>();
            this.number = new Regex(@"-?((0x[0-9a-f]+)|([0-9]+(\.[0-9]+)?(e(\+|\-)?[0-9]+)?))", RegexOptions.IgnoreCase);
        }

        public Maybe<Token> Next()
        {
            var token = head.Any()
                ? Maybe<Token>.Some(head.Pop())
                : source.Next();

            if (!token.HasValue || !token.Value.Is(TokenType.Unparsed))
            {
                return token;
            }

            var content = token.Value.Content;
            var lookup = number.Match(content);
            if (!lookup.Success || IsSurroundedByIdentifierChar(content, lookup.Index, lookup.Index + lookup.Length))
            {
                return token;
            }

            var start = lookup.Index;
            var end = start + lookup.Length;
            if (end < content.Length)
            {
                head.Push(new Token(token.Value.Line, token.Value.Column + end, TokenType.Unparsed, content.Substring(end)));
            }

            var type = lookup.Value.Contains(".") ? TokenType.Float : TokenType.Int;
            head.Push(new Token(token.Value.Line, token.Value.Column + start, type, lookup.Value));

            if (start > 0)
            {
                head.Push(new Token(token.Value.Line, token.Value.Column, TokenType.Unparsed, content.Substring(0, start)));
            }

            return Next();
        }

        private bool IsSurroundedByIdentifierChar(string content, int start, int end)
        {
            if (start > 0 && IsIdentifierChar(content[start - 1]))
            {
                return true;
            }
            else if (end < content.Length && IsIdentifierChar(content[end]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsIdentifierChar(char value)
        {
            return char.IsLetterOrDigit(value) || value == '_';
        }

        public void Reset()
        {
            head.Clear();
            source.Reset();
        }
    }
}
