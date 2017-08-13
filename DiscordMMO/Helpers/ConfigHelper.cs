using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DiscordMMO.Helpers
{
    public static class ConfigHelper
    {

        private static string path;

        public static void SetConfigPath(string path)
        {
            ConfigHelper.path = path;
        }

        public static string GetValue(string key)
        {
            StreamReader file = new StreamReader(path);
            string line;

            using (file)
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (line.StartsWith(key + "="))
                    {
                        return line.Split('=')[1];
                    }
                }
            }
            return "";

        }

    }
}
