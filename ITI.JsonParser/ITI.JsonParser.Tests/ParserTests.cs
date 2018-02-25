using System;
using System.Collections.Generic;
using NUnit.Framework;
using FluentAssertions;
using ITI.JsonParser;

namespace ITI.JsonParser.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [TestCase(@"null")]
        public void test10_parse_null(string value)
        {
            int start = 0;
            int count = value.Length;
            Parser.ParseNull(value, ref start, ref count).Should().BeNull();
        }

        [TestCase(@"nul")]
        public void test11_parse_invalid_null(string value)
        {
            int start = 0;
            int count = value.Length;
            Action action = () => Parser.ParseNull(value, ref start, ref count);
            action.Should().Throw<FormatException>();
        }

        [TestCase(@"true")]
        public void test20_parse_boolean(string value)
        {
            int start = 0;
            int count = value.Length;
            Parser.ParseBoolean(value, ref start, ref count).Should().BeTrue();
        }

        [TestCase(@"truth")]
        public void test21_parse_invalid_boolean(string value)
        {
            int start = 0;
            int count = value.Length;
            Action action = () => Parser.ParseBoolean(value, ref start, ref count);
            action.Should().Throw<FormatException>();
        }

        [TestCase(@"12.99")]
        public void test30_parse_double(string value)
        {
            int start = 0;
            int count = value.Length;
            Parser.ParseDouble(value, ref start, ref count).Should().Be(12.99d);
        }

        [TestCase(@"-12.99")]
        public void test31_parse_negative_double(string value)
        {
            int start = 0;
            int count = value.Length;
            Parser.ParseDouble(value, ref start, ref count).Should().Be(-12.99d);
        }

        [TestCase(@"-12.k9")]
        public void test32_parse_invalid_double(string value)
        {
            int start = 0;
            int count = value.Length;
            Action action = () => Parser.ParseDouble(value, ref start, ref count);
            action.Should().Throw<FormatException>();
        }

        [TestCase(@"""""")]
        public void test40_parse_empty_string(string value)
        {
            int start = 0;
            int count = value.Length;
            Parser.ParseString(value, ref start, ref count).Should().Be("");
        }

        [TestCase(@"""hello""")]
        public void test41_parse_string(string value)
        {
            int start = 0;
            int count = value.Length;
            Parser.ParseString(value, ref start, ref count).Should().Be("hello");
        }

        [TestCase(@"""To be\u002C \u006Fr n\u006Ft t\u006F be""")]
        public void test42_parse_string_with_hexadecimals(string value)
        {
            int start = 0;
            int count = value.Length;
            Parser.ParseString(value, ref start, ref count).Should().BeEquivalentTo("To be, or not to be");
        }

        [TestCase(@"""William Shakespeare : \""To be, or not to be\""""")]
        public void test43_parse_escaped_string(string value)
        {
            int start = 0;
            int count = value.Length;
            Parser.ParseString(value, ref start, ref count).Should().BeEquivalentTo("William Shakespeare : \\\"To be, or not to be\\\"");
        }

        [TestCase(@"[]")]
        public void test50_parse_empty_array(string value)
        {
            int start = 0;
            int count = value.Length;
            Parser.ParseArray(value, ref start, ref count).Should().NotBeNull();
        }

        [TestCase(@"[""Elliot"","""",null,""Elliot"","""",null]")]
        public void test51_parse_array_with_values(string value)
        {
            int start = 0;
            int count = value.Length;
            Parser.ParseArray(value, ref start, ref count).Length.Should().Be(6);
        }

        [TestCase(@"{}")]
        public void test60_parse_empty_object(string value)
        {
            int start = 0;
            int count = value.Length;
            Parser.ParseObject(value, ref start, ref count).Should().NotBeNull();
        }

        [TestCase(@"{""Joseph"":""Heller""}")]
        public void test61_parse_object_with_pair_value(string value)
        {
            int start = 0;
            int count = value.Length;
            Parser.ParseObject(value, ref start, ref count).Count.Should().Be(1);
        }

        [TestCase(@"{""active"":true,""age"":20,""salutation"":""hello"",""array"":[""Elliot"","""",null],""object"":{""Joseph"":""Heller""}}")]
        public void test62_parse_object_with_differents_values(string value)
        {
            int start = 0;
            int count = value.Length;
            Parser.ParseObject(value, ref start, ref count).Count.Should().Be(5);
        }

        [TestCase(@"{""active"":true,""age"":20,""active"":""false""}")]
        public void test63_duplicate_key_in_object_error(string value)
        {
            int start = 0;
            int count = value.Length;
            Action action = () => Parser.ParseObject(value, ref start, ref count);
            action.Should().Throw<InvalidOperationException>();
        }

        [TestCase(@"
            {   ""active"" : true       ,
                ""age"" : 20    ,
                ""salutation"" : ""hello"",  
                ""Null"" :   null  ,
                ""self"": 
                    {   ""active""  : true  ,
                        ""age"" :  20  ,  
                        ""salutation"":""hello""
                    }
            }")]
        public void test70_parse_object_with_spaces(string value)
        {
            int start = 0;
            int count = value.Length;
            Parser.ParseObject(value, ref start, ref count).Count.Should().Be(5);
        }
    }
}
