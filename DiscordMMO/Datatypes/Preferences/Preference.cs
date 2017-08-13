using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.Preferences
{
    public class Preference<T> : IPreference
    {

        public Preference(T value)
        {
            this.value = value;
        }

        public T value { get; set; }

        public Type type { get { return typeof(T); } }

        public override string ToString()
        {
            return value.ToString();
        }

        public static implicit operator Preference<T>(T value)
        {
            return new Preference<T>(value);
        }

        public static implicit operator T(Preference<T> value)
        {
            return value.value;
        }

    }
}
