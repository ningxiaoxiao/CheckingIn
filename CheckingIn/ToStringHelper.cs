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
            if (t.Ticks <= 0)//使用特别值 进行空值处理
                return "";
            return t.ToString();
        }
    }
}
