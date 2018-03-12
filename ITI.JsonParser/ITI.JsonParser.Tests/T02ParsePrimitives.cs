using System;
using System.Collections.Generic;
using NUnit.Framework;
using FluentAssertions;
using ITI.JsonParser;

namespace ITI.JsonParser.Tests
{
    [TestFixture]
    class T02ParsePrimitives
    {
        [TestCase( @"null" )]
        public void test_2_01_parse_null( string value )
        {
            object _parsed;
            int _start = 0;
            int _count = value.Length;

            _parsed = Parser.ParseNull( value, ref _start, ref _count );

            _parsed.Should().BeNull();
        }

        [TestCase( @"nul" )]
        public void test_2_02_parse_invalid_null( string value )
        {
            int _start = 0;
            int _count = value.Length;

            Action action = () => Parser.ParseNull( value, ref _start, ref _count );

            action.Should().Throw<FormatException>();
        }

        [TestCase( @"true" )]
        public void test_2_03_parse_boolean( string value )
        {
            bool _parsed;
            int _start = 0;
            int _count = value.Length;

            _parsed = Parser.ParseBoolean( value, ref _start, ref _count );

            _parsed.Should().BeTrue();
        }

        [TestCase( @"truth" )]
        public void test_2_04_parse_invalid_boolean( string value )
        {
            int _start = 0;
            int _count = value.Length;

            Action action = () => Parser.ParseBoolean( value, ref _start, ref _count );

            action.Should().Throw<FormatException>();
        }

        [TestCase( @"12" )]
        public void test_2_05_parse_int( string value )
        {
            int _parsed;
            int _start = 0;
            int _count = value.Length;

            _parsed = Parser.ParseInt( value, ref _start, ref _count );

            _parsed.Should().Be( 12 );
        }

        [TestCase( @"12.99" )]
        public void test_2_06_parse_invalid_int( string value )
        {
            int _start = 0;
            int _count = value.Length;

            Action action = () => Parser.ParseInt( value, ref _start, ref _count );

            action.Should().Throw<FormatException>();
        }

        [TestCase( @"-12" )]
        public void test_2_07_parse_negative_int( string value )
        {
            int _parsed;
            int _start = 0;
            int _count = value.Length;

            _parsed = Parser.ParseInt( value, ref _start, ref _count );

            _parsed.Should().Be( -12 );
        }

        [TestCase( @"""""" )]
        public void test_2_08_parse_empty_string( string value )
        {
            string _parsed;
            int _start = 0;
            int _count = value.Length;

            _parsed = Parser.ParseString( value, ref _start, ref _count );

            _parsed.Should().Be( "" );
        }

        [TestCase( @"""hello""" )]
        public void test_2_09_parse_string( string value )
        {
            string _parsed;
            int _start = 0;
            int _count = value.Length;

            _parsed = Parser.ParseString( value, ref _start, ref _count );

            _parsed.Should().Be( "hello" );
        }

        [TestCase( @"""William Shakespeare : \""To be, or not to be\""""" )]
        public void test_2_10_parse_escaped_string( string value )
        {
            string _parsed;
            int _start = 0;
            int _count = value.Length;

            _parsed = Parser.ParseString( value, ref _start, ref _count );

            _parsed.Should().BeEquivalentTo( "William Shakespeare : \\\"To be, or not to be\\\"" );
        }

        [TestCase( @"""To be\u002C \u006Fr n\u006Ft t\u006F be""" )]
        public void test_2_11_parse_string_with_unicode( string value )
        {
            string _parsed;
            int _start = 0;
            int _count = value.Length;

            _parsed = Parser.ParseString( value, ref _start, ref _count );

            _parsed.Should().BeEquivalentTo( "To be, or not to be" );
        }
    }
}
