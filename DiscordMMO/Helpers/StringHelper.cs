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

    }
}
