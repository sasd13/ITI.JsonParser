using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.JsonParser
{
    public static class Parser
    {
        public static object ParseNull( string value, ref int start, ref int count )
        {
            throw new NotImplementedException();
        }

        public static bool ParseBoolean( string value, ref int start, ref int count )
        {
            throw new NotImplementedException();
        }

        public static int ParseInt( string value, ref int start, ref int count )
        {
            throw new NotImplementedException();
        }

        public static string ParseString( string value, ref int start, ref int count )
        {
            throw new NotImplementedException();
        }

        public static object[] ParseArray( string value, ref int start, ref int count )
        {
            throw new NotImplementedException();
        }

        public static Dictionary<string, object> ParseObject( string value, ref int start, ref int count )
        {
            throw new NotImplementedException();
        }

        public static bool TryReadNextChar( string value, ref int start, ref int count, out char current )
        {
            throw new NotImplementedException();
        }

        public static bool TryReadNonBlankChar( string value, ref int start, ref int count, out char current )
        {
            throw new NotImplementedException();
        }
    }
}
