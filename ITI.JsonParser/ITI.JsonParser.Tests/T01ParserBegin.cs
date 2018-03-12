using System;
using System.Collections.Generic;
using NUnit.Framework;
using FluentAssertions;
using ITI.JsonParser;

namespace ITI.JsonParser.Tests
{
    [TestFixture]
    public class T01ParserBegin
    {
        /**
         * to test method, that moves the cursor in the json string on the next character;
         * @return true, and places the cursor on the next character of the string if possible
         * @return false, if the next character is out of range of the json string,
         *                 or if the "_start" position is the last character in the string "value"
         */

        [TestCase( @"true" )]
        public void test_1_01_move_next( string value )
        {
            bool _read = false;
            int _start = 0;
            int _count = value.Length;
            char _current;

            _read = Parser.TryReadNextChar( value, ref _start, ref _count, out _current );

            _read.Should().BeTrue();
            _start.Should().Be( 1 );
            _count.Should().Be( value.Length - 1 );
            _current.Should().Be( 'r' );
        }

        [TestCase( @"false" )]
        public void test_1_02_move_next_when_end_reached( string value )
        {
            bool _read = false;
            int _start = 3;
            int _count = 1;
            char _current;

            Parser.TryReadNextChar( value, ref _start, ref _count, out _current );

            _start.Should().Be( 4 );
            _count.Should().Be( 0 );

            // end of the json string is reached

            _read = Parser.TryReadNextChar( value, ref _start, ref _count, out _current );

            _read.Should().BeFalse();
        }

        [TestCase( @"abcd{""simple"":""json""}xyz" )]
        public void test_1_03_move_next_json_beginning( string value )
        {
            bool _read = false;
            int _start = 4;  // beginning of the json string: curly bracket character 
            int _count = 17; // length of the json string: {"simple":"json"}
            char _current;

            _read = Parser.TryReadNextChar( value, ref _start, ref _count, out _current );

            _read.Should().BeTrue();
            _start.Should().Be( 5 );
            _count.Should().Be( 16 );
            _current.Should().Be( '"' );
        }

        [TestCase( @"abcd{""simple"":""json""}xyz" )]
        public void test_1_04_move_next_json_end_reached( string value )
        {
            bool _read = false;
            int _start = 4;  // end of the json string: curly bracket character
            int _count = 17; // length of the json string: {"simple":"json"}
            char _current;

            // move the cursor till the end of the json string
            for( int i = 0; i <= 17; i++ )
            {
                Parser.TryReadNextChar( value, ref _start, ref _count, out _current );
            }

            _read = Parser.TryReadNextChar( value, ref _start, ref _count, out _current );

            _read.Should().BeFalse();
            _start.Should().Be( 21 );
            _count.Should().Be( 0 );
        }

        [TestCase( @"  true" )]
        public void test_1_05_skip_spaces( string value )
        {
            bool _read = false;
            int _start = 0;
            int _count = value.Length;
            char _current;

            _read = Parser.TryReadNonBlankChar( value, ref _start, ref _count, out _current );

            _read.Should().BeTrue();
            _start.Should().Be( 2 );
            _count.Should().Be( value.Length - 2 );
            _current.Should().Be( 't' );
        }
    }
}
