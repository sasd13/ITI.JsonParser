using System;
using System.Collections.Generic;
using NUnit.Framework;
using FluentAssertions;
using ITI.JsonParser;

namespace ITI.JsonParser.Tests
{
    [TestFixture]
    class T03ParseStructures
    {
        [TestCase( @"[]" )]
        public void test_3_01_parse_empty_array( string value )
        {
            object[] _parsed;
            int _start = 0;
            int _count = value.Length;

            _parsed = Parser.ParseArray( value, ref _start, ref _count );

            _parsed.Should().NotBeNull();
            _parsed.Length.Should().Be( 0 );
        }

        [TestCase( @"[""Elliot"","""",null,""Elliot"","""",null]" )]
        public void test_3_02_parse_array_with_values( string value )
        {
            object[] _parsed;
            int _start = 0;
            int _count = value.Length;

            _parsed = Parser.ParseArray( value, ref _start, ref _count );

            _parsed.Length.Should().Be( 6 );
        }

        [TestCase( @"
            [
                ""Elliot""   ,
                """"    ,
                null   ,
                ""Elliot""   ,
                """"   ,
                null
            ]
        " )]
        public void test_3_03_parse_array_with_spaces( string value )
        {
            object[] _parsed;
            int _start = 0;
            int _count = value.Length;

            _parsed = Parser.ParseArray( value, ref _start, ref _count );

            _parsed.Length.Should().Be( 6 );
        }

        [TestCase( @"{}" )]
        public void test_3_04_parse_empty_object( string value )
        {
            Dictionary<string, object> _parsed;
            int _start = 0;
            int _count = value.Length;

            _parsed = Parser.ParseObject( value, ref _start, ref _count );

            _parsed.Should().NotBeNull();
            _parsed.Count.Should().Be( 0 );
        }

        [TestCase( @"{""Joseph"":""Heller""}" )]
        public void test_3_05_parse_object_with_pair_value( string value )
        {
            Dictionary<string, object> _parsed;
            int _start = 0;
            int _count = value.Length;

            _parsed = Parser.ParseObject( value, ref _start, ref _count );

            _parsed.Count.Should().Be( 1 );
        }

        [TestCase( @"{""active"":true,""age"":20,""salutation"":""hello""}" )]
        public void test_3_06_parse_object_with_differents_values( string value )
        {
            Dictionary<string, object> _parsed;
            int _start = 0;
            int _count = value.Length;

            _parsed = Parser.ParseObject( value, ref _start, ref _count );

            _parsed.Count.Should().Be( 3 );
        }

        [TestCase( @"{""active"":true,""age"":20,""active"":""false""}" )]
        public void test_3_07_duplicate_key_in_object_error( string value )
        {
            int _start = 0;
            int _count = value.Length;

            Action action = () => Parser.ParseObject( value, ref _start, ref _count );

            action.Should().Throw<InvalidOperationException>();
        }

        [TestCase( @"
            {
                ""active"" : true       ,
                ""age"" : 20    ,
                ""salutation"" : ""hello""   ,  
                ""name"" :   null
            }
        " )]
        public void test_3_08_parse_object_with_spaces( string value )
        {
            Dictionary<string, object> _parsed;
            int _start = 0;
            int _count = value.Length;

            _parsed = Parser.ParseObject( value, ref _start, ref _count );

            _parsed.Count.Should().Be( 4 );
        }

        [TestCase( @"
            {
                ""active"" : true,
                ""age"" : 20,
                ""salutation"" : ""hello"",  
                ""name"" : null,
                ""weekend"": 
                    [
                        ""saturday"",
                        ""sunday""
                    ]
            }
        " )]
        public void test_3_09_parse_object_with_nested_array( string value )
        {
            Dictionary<string, object> _parsed;
            object[] _nested1_array;
            int _start = 0;
            int _count = value.Length;

            _parsed = Parser.ParseObject( value, ref _start, ref _count );
            _nested1_array = (object[])_parsed["weekend"];

            _parsed.Count.Should().Be( 5 );
            _nested1_array.Length.Should().Be( 2 );
        }

        [TestCase( @"
            {
                ""active"" : true,
                ""age"" : 20,
                ""salutation"" : ""hello"",  
                ""name"" : null,
                ""address"": 
                    {
                        ""number"" : ""1"",
                        ""street"" : ""Rue de la Paix"",
                        ""city"" : ""Paris""
                    }
            }
        " )]
        public void test_3_10_parse_object_with_nested_object( string value )
        {
            Dictionary<string, object> _parsed, _nested1_object;
            int _start = 0;
            int _count = value.Length;

            _parsed = Parser.ParseObject( value, ref _start, ref _count );
            _nested1_object = (Dictionary<string, object>)_parsed["address"];

            _parsed.Count.Should().Be( 5 );
            _nested1_object.Count.Should().Be( 3 );
        }

        [TestCase( @"
            {
                ""active"" : true,
                ""age"" : 20,
                ""salutation"" : ""hello"",  
                ""name"" : null,
                ""user"": 
                    {
                        ""firstname"" : ""William"",
                        ""lastname"" : ""Shakespeare"",
                        ""age"" : 20,
                        ""quote"" : ""\""To be\u002C \u006Fr n\u006Ft t\u006F be\"""",
                        ""address"":
                            {
                                ""number"" : ""1"",
                                ""street"" : ""Rue de la Paix"",
                                ""city"" : ""Paris""
                            },
                        ""daysoff"" :
                            [
                                ""monday"",
                                ""tuesday"",
                                ""wednesday"",
                                ""thursday""
                            ],
                        ""workdays"" :
                            [
                                {
                                    ""day"" : ""friday"",
                                    ""payroll"" : 100
                                },
                                {
                                    ""day"" : ""saturday"",
                                    ""payroll"" : 150
                                },
                                {
                                    ""day"" : ""sunday"",
                                    ""payroll"" : 200
                                },
                            ]
                    }
            }
        " )]
        public void test_3_11_parse_complex_object( string value )
        {
            Dictionary<string, object> _parsed, _user, _address;
            object[] _daysoff, _workdays;
            string _quote;
            int _payroll;
            int _start = 0;
            int _count = value.Length;

            _parsed = Parser.ParseObject( value, ref _start, ref _count );
            _user = (Dictionary<string, object>)_parsed["user"];
            _quote = (string)_user["quote"];
            _address = (Dictionary<string, object>)_user["address"];
            _daysoff = (object[])_user["daysoff"];
            _workdays = (object[])_user["workdays"];
            _payroll = (int)((Dictionary<string, object>)_workdays[2])["payroll"];

            _parsed.Count.Should().Be( 5 );
            _user.Count.Should().Be( 7 );
            _quote.Should().BeEquivalentTo( "\\\"To be, or not to be\\\"" );
            _address.Count.Should().Be( 3 );
            _daysoff.Length.Should().Be( 4 );
            _workdays.Length.Should().Be( 3 );
            _payroll.Should().Be( 200 );
        }
    }
}
