using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Helpers
{
    public static class StringHelper
    {

        public static string FormatTimeSpan(TimeSpan d)
        {
            return d.ToString(@"dd\:HH\:mm\:ss");
        }

        // Stolen from https://stackoverflow.com/questions/4135317/make-first-letter-of-a-string-upper-case-with-maximum-performance/4135491#4135491
        public static string CapitalizeFirst(this string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }
            

    }
}
