using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckingIn
{
    static class ToStringHelper
    {
        public static string ToMyString(this TimeSpan t)
        {
            if (t.Ticks < 0)
                return "";
            return t.ToString();
        }
    }
}
