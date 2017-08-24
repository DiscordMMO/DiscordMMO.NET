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

        [Obsolete("This is currently a low priority feature")]
        public static string CreateInsertOrOverwriteString(string[] param)
        {
            // TODO: Implement CreateInstertOrOverwriteString
            return "";
        }

    }
}
