using ElmRuntime2.Exceptions;
using ElmRuntime2.Expressions;
using ElmRuntime2.Lexer;
using ElmRuntime2.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Parser
{
    public static class PatternParser
    {
        public static ParseResult<Pattern> ParsePattern(TokenStream stream, int position)
        {
            if (position >= stream.Length)
            {
                return new ParseResult<Pattern>(false, default(Pattern), position);
            }

            var pattern = default(Pattern);

            if (stream.IsAt(position, TokenType.Underscore))
            {
                pattern = new UnderscorePattern();
                position++;
            }
            else if (stream.IsAt(position, TokenType.Identifier) && ParserHelper.IsCtorName(stream, position))
            {
                var parsed = ParseUnionPattern(stream, position);
                pattern = parsed.Value;
                position = parsed.Position;
            }
            else if (stream.IsAt(position, TokenType.Identifier) && ParserHelper.IsVariableName(stream, position))
            {
                var name = stream.At(position).Content;
                pattern = new VariablePattern(name);
                position++;
            }
            else if (stream.IsAt(position, TokenType.LeftBracket))
            {
                var parsed = ParseListPattern(stream, position);
                pattern = parsed.Value;
                position = parsed.Position;
            }
            else if (stream.IsAt(position, TokenType.LeftParen))
            {
                var parsed = ParseTupleOrParenthesizedPattern(stream, position);
                pattern = parsed.Value;
                position = parsed.Position;
            }
            else if (stream.IsAt(position, TokenType.LeftBrace))
            {
                var parsed = ParseRecordPattern(stream, position);
                pattern = parsed.Value;
                position = parsed.Position;
            }
            else if (stream.IsAnyAt(position, TokenType.True, TokenType.False, TokenType.Int, TokenType.Float, TokenType.String, TokenType.Char))
            {
                var parsed = ValueParser.ParseValue(stream, position);
                pattern = new LiteralPattern(parsed.Value);
                position = parsed.Position;
            }
            else
            {
                throw new NotSupportedException($"Encountered unexpected token while parsing pattern");
            }

            if (stream.IsAt(position, TokenType.As, TokenType.Identifier))
            {
                var alias = stream.At(position + 1).Content;
                pattern = new AliasPattern(pattern, alias);
                position += 2;
            }

            if (stream.IsAt(position, "::"))
            {
                var parsed = ParseConstructorListPattern(stream, position, pattern);
                pattern = parsed.Value;
                position = parsed.Position;
            }

            return new ParseResult<Pattern>(pattern, position);
        }

        private static ParseResult<Pattern> ParseUnionPattern(TokenStream stream, int position)
        {
            var constructor = stream.At(position).Content;
            position++;

            var values = new List<Pattern>();
            while (!stream.IsAt(position, TokenType.Arrow))
            {
                var parsed = ParsePattern(stream, position);
                values.Add(parsed.Value);
                position = parsed.Position;
            }

            return new ParseResult<Pattern>(
                new UnionConstructorPattern(constructor, values.ToArray()), position
            );
        }

        private static ParseResult<Pattern> ParseConstructorListPattern(TokenStream stream, int position, Pattern head)
        {
            var tailParsed = ParsePattern(stream, position + 1);

            return new ParseResult<Pattern>(
                new ListHeadTailPattern(head, tailParsed.Value), tailParsed.Position
            );
        }
        
        private static ParseResult<Pattern> ParseListPattern(TokenStream stream, int position)
        {
            var items = new List<Pattern>();

            var elements = ParserHelper.ParseArray(stream, position);
            foreach(var element in elements.Value)
            {
                var parsedElement = ParsePattern(element, 0);
                items.Add(parsedElement.Value);
            }

            return new ParseResult<Pattern>(new ListPattern(items.ToArray()), elements.Position);
        }

        private static ParseResult<Pattern> ParseTupleOrParenthesizedPattern(TokenStream stream, int position)
        {
            var elements = ParserHelper.ParseArray(stream, position);
            if (elements.Value.Length == 1)
            {
                return ParsePattern(elements.Value[0], elements.Position);
            }

            var items = new List<Pattern>();
            foreach (var element in elements.Value)
            {
                var parsedElement = ParsePattern(element, 0);
                items.Add(parsedElement.Value);
            }

            return new ParseResult<Pattern>(new TuplePattern(items.ToArray()), elements.Position);
        }

        private static ParseResult<Pattern> ParseRecordPattern(TokenStream stream, int position)
        {
            var elements = ParserHelper.ParseArray(stream, position);

            var fields = new List<Pattern>();
            foreach (var element in elements.Value)
            {
                var parsedElement = ParsePattern(element, 0);
                fields.Add(parsedElement.Value);
            }

            return new ParseResult<Pattern>(new RecordPattern(fields.ToArray()), elements.Position);
        }
    }
}
