using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ITI.JsonParser
{

    public static class Parser
    {
        private static CultureInfo _culture = CultureInfo.CreateSpecificCulture("en-US");
        private static Regex _regex = new Regex(@"\\u(?<Value>[a-zA-Z0-9]{4})", RegexOptions.Compiled);

        public static object ParseNull(string value, ref int start, ref int count)
        {
            char _current = SkipSpaces(value, ref start, ref count);
            Debug.Assert(_current.Equals('n'));
            if (!ReadNonStringValue(value, ref start, ref count).Equals("null"))
            {
                throw new FormatException();
            }
            return null;
        }

        public static bool ParseBoolean(string value, ref int start, ref int count)
        {
            char _current = SkipSpaces(value, ref start, ref count);
            Debug.Assert(_current.Equals('t') || _current.Equals('f'));
            return Boolean.Parse(ReadNonStringValue(value, ref start, ref count));
        }

        public static double ParseDouble(string value, ref int start, ref int count)
        {
            char _current = SkipSpaces(value, ref start, ref count);
            Debug.Assert(_current.Equals('-') || Int32.TryParse(_current.ToString(), out int _result));
            return Double.Parse(ReadNonStringValue(value, ref start, ref count), NumberStyles.Number, _culture);
        }

        public static string ParseString(string value, ref int start, ref int count)
        {
            char _current = SkipSpaces(value, ref start, ref count);
            Debug.Assert(_current.Equals('"'));
            return Decoder(ReadStringValue(value, ref start, ref count));
        }

        public static object[] ParseArray(string value, ref int start, ref int count)
        {
            char _current = SkipSpaces(value, ref start, ref count);
            Debug.Assert(_current.Equals('['));
            List<object> _results = new List<object>();
            while (MoveNext(value, ref start, ref count, out _current))
            {
                _current = SkipSpaces(value, ref start, ref count);
                if (!_current.Equals(']'))
                {
                    _results.Add(ParseValue(value, ref start, ref count, _current));
                    _current = SkipSpaces(value, ref start, ref count);
                }

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
            char _current = SkipSpaces(value, ref start, ref count);
            Debug.Assert(_current.Equals('{'));
            Dictionary<string, object> _results = new Dictionary<string, object>();
            while (MoveNext(value, ref start, ref count, out _current))
            {
                _current = SkipSpaces(value, ref start, ref count);
                if (!_current.Equals('}'))
                {
                    AddProperty(value, ref start, ref count, _results);
                    _current = SkipSpaces(value, ref start, ref count);
                }

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

        private static void AddProperty(string value, ref int start, ref int count, Dictionary<string, object> _results)
        {
            char _current = SkipSpaces(value, ref start, ref count);
            if (!_current.Equals('"'))
            {
                throw new FormatException();
            }

            string _key = ReadStringValue(value, ref start, ref count);
            if (_results.ContainsKey(_key))
            {
                throw new InvalidOperationException();
            }

            _current = SkipSpaces(value, ref start, ref count);
            if (!_current.Equals(':'))
            {
                throw new FormatException();
            }

            if (MoveNext(value, ref start, ref count))
            {
                _current = SkipSpaces(value, ref start, ref count);
                _results.Add(_key, ParseValue(value, ref start, ref count, _current));
            }
        }

        private static object ParseValue(string value, ref int start, ref int count, char current)
        {
            switch (current)
            {
                case '"': return ParseString(value, ref start, ref count);
                case 'n': return ParseNull(value, ref start, ref count);
                case 't': case 'f': return ParseBoolean(value, ref start, ref count);
                case '[': return ParseArray(value, ref start, ref count);
                case '{': return ParseObject(value, ref start, ref count);
                default:
                    if (current.Equals('-') || Int32.TryParse(current.ToString(), out int _result))
                    {
                        return ParseDouble(value, ref start, ref count);
                    }
                    else
                    {
                        throw new FormatException();
                    }
            }
        }

        private static bool MoveNext(string value, ref int start, ref int count)
        {
            return MoveNext(value, ref start, ref count, out char _current);
        }

        private static bool MoveNext(string value, ref int start, ref int count, out char current)
        {
            if (count <= 0 || start >= value.Length - 1)
            {
                current = start < value.Length ? value[start] : '\0';
                return false;
            }

            start += 1;
            count -= 1;
            current = value[start];
            return true;
        }

        private static char SkipSpaces(string value, ref int start, ref int count)
        {
            char _current = value[start];
            while (_current.ToString().Trim().Length == 0 && MoveNext(value, ref start, ref count, out _current))
            {
                //Do nothing... just skipping white spaces
            }
            return _current;
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
            } while (MoveNext(value, ref start, ref count, out _current));
            return _builder.ToString().Trim();
        }

        private static string ReadStringValue(string value, ref int start, ref int count)
        {
            StringBuilder _builder = new StringBuilder();
            char _previous, _current;
            while (MoveNext(value, ref start, ref count, out _current))
            {
                _previous = value[start - 1];
                if (_current.Equals('"') && !_previous.Equals('\\'))
                {
                    //Go to next delimiter char : ',' or ']' or '}'
                    if (MoveNext(value, ref start, ref count))
                    {
                        SkipSpaces(value, ref start, ref count);
                    }

                    break;
                }

                _builder.Append(_current);
            }
            return _builder.ToString();
        }

        private static string Decoder(string value)
        {
            return _regex.Replace(
                value,
                m => ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString()
            );
        }
    }
}
