using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.Preferences
{
    public class Preference
    {

        public Preference() { }

        public Preference(object value)
        {
            this.value = value;
        }

        public object value { get; set; }

        public override string ToString()
        {
            return value.ToString();
        }

        public static Preference GetPreference(object value)
        {
            return new Preference(value);
        }

    }
}
