using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ITI.JsonParser
{
    public static class Parser
    {
        private static Regex _regex = new Regex(@"\\u(?<Value>[a-zA-Z0-9]{4})", RegexOptions.Compiled);

        public static object ParseNull(string value, ref int start, ref int count)
        {
            TryReadNonBlankChar(value, ref start, ref count, out char _current);
            if (!_current.Equals('n')
                || !ReadNonStringValue(value, ref start, ref count).Equals("null"))
            {
                throw new FormatException();
            }

            return null;
        }

        public static bool ParseBoolean(string value, ref int start, ref int count)
        {
            TryReadNonBlankChar(value, ref start, ref count, out char _current);
            if ((!_current.Equals('t') && !_current.Equals('f'))
                || !Boolean.TryParse(ReadNonStringValue(value, ref start, ref count), out bool _result))
            {
                throw new FormatException();
            }

            return _result;
        }

        public static int ParseInt(string value, ref int start, ref int count)
        {
            TryReadNonBlankChar(value, ref start, ref count, out char _current);
            if ((!_current.Equals('-') && !Int32.TryParse(_current.ToString(), out int _void))
                || !Int32.TryParse(ReadNonStringValue(value, ref start, ref count), out int _result))
            {
                throw new FormatException();
            }

            return _result;
        }

        public static string ParseString(string value, ref int start, ref int count)
        {
            TryReadNonBlankChar(value, ref start, ref count, out char _current);
            if (!_current.Equals('"'))
            {
                throw new FormatException();
            }

            return Decoder(ReadStringValue(value, ref start, ref count));
        }

        public static object[] ParseArray(string value, ref int start, ref int count)
        {
            TryReadNonBlankChar(value, ref start, ref count, out char _current);
            if (!_current.Equals('['))
            {
                throw new FormatException();
            }

            List<object> _results = new List<object>();
            while (MoveNext(value, ref start, ref count))
            {
                TryReadNonBlankChar(value, ref start, ref count, out _current);
                if (!_current.Equals(']'))
                {
                    _results.Add(ParseValue(value, ref start, ref count, _current));
                }

                TryReadNonBlankChar(value, ref start, ref count, out _current);
                if (_current.Equals(']'))
                {
                    MoveNext(value, ref start, ref count);
                    break;
                }
                else if (!_current.Equals(','))
                {
                    throw new FormatException();
                }
            }

            return _results.ToArray();
        }

        public static Dictionary<string, object> ParseObject(string value, ref int start, ref int count)
        {
            TryReadNonBlankChar(value, ref start, ref count, out char _current);
            if (!_current.Equals('{'))
            {
                throw new FormatException();
            }

            Dictionary<string, object> _results = new Dictionary<string, object>();
            while (MoveNext(value, ref start, ref count))
            {
                TryReadNonBlankChar(value, ref start, ref count, out _current);
                if (!_current.Equals('}'))
                {
                    AddProperty(value, ref start, ref count, _results);
                }

                TryReadNonBlankChar(value, ref start, ref count, out _current);
                if (_current.Equals('}'))
                {
                    MoveNext(value, ref start, ref count);
                    break;
                }
                else if (!_current.Equals(','))
                {
                    throw new FormatException();
                }
            }

            return _results;
        }

        private static bool TryReadNextChar(string value, ref int start, ref int count, out char current)
        {
            if (count <= 0 || start >= value.Length - 1)
            {
                current = start < value.Length ? value[start] : '\0';
                return false;
            }

            start++;
            count--;
            current = value[start];

            return true;
        }

        private static bool TryReadNonBlankChar(string value, ref int start, ref int count, out char current)
        {
            bool _hasRead = false;
            current = value[start];
            while (current.ToString().Trim().Length == 0 && TryReadNextChar(value, ref start, ref count, out current))
            {
                _hasRead = true;
            }

            return _hasRead;
        }

        private static string ReadNonStringValue(string value, ref int start, ref int count)
        {
            StringBuilder _builder = new StringBuilder();
            char _current = value[start];
            do
            {
                if (_current.Equals(',') || _current.Equals(']') || _current.Equals('}'))
                {
                    break;
                }

                _builder.Append(_current);
            } while (TryReadNextChar(value, ref start, ref count, out _current));

            return _builder.ToString().Trim();
        }

        private static string ReadStringValue(string value, ref int start, ref int count)
        {
            StringBuilder _builder = new StringBuilder();
            char _current;
            while (TryReadNextChar(value, ref start, ref count, out _current))
            {
                if (_current.Equals('"') && !value[start - 1].Equals('\\'))
                {
                    MoveNext(value, ref start, ref count);
                    break;
                }

                _builder.Append(_current);
            }

            return _builder.ToString();
        }

        private static object ParseValue(string value, ref int start, ref int count, char current)
        {
            switch (current)
            {
                case 'n': return ParseNull(value, ref start, ref count);
                case 't': case 'f': return ParseBoolean(value, ref start, ref count);
                case '"': return ParseString(value, ref start, ref count);
                case '[': return ParseArray(value, ref start, ref count);
                case '{': return ParseObject(value, ref start, ref count);
                default:
                    if (current.Equals('-') || Int32.TryParse(current.ToString(), out int _void))
                    {
                        return ParseInt(value, ref start, ref count);
                    }
                    else
                    {
                        throw new FormatException();
                    }
            }
        }

        private static bool MoveNext(string value, ref int start, ref int count)
        {
            return TryReadNextChar(value, ref start, ref count, out char _void);
        }

        private static string Decoder(string value)
        {
            return _regex.Replace(
                value,
                m => ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString()
            );
        }

        private static void AddProperty(string value, ref int start, ref int count, Dictionary<string, object> _results)
        {
            TryReadNonBlankChar(value, ref start, ref count, out char _current);
            if (!_current.Equals('"'))
            {
                throw new FormatException();
            }

            string _key = ReadStringValue(value, ref start, ref count);
            if (_results.ContainsKey(_key))
            {
                throw new InvalidOperationException();
            }

            TryReadNonBlankChar(value, ref start, ref count, out _current);
            if (!_current.Equals(':'))
            {
                throw new FormatException();
            }

            if (MoveNext(value, ref start, ref count))
            {
                TryReadNonBlankChar(value, ref start, ref count, out _current);
                _results.Add(_key, ParseValue(value, ref start, ref count, _current));
            }
        }
    }
}
