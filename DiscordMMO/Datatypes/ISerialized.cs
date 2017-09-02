using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes
{
    public interface ISerialized
    {
        string convertPrefix { get; }
    }

    [AttributeUsage((AttributeTargets.Field | AttributeTargets.Property), AllowMultiple = false, Inherited = true)]
    public class SerializedAttribute : Attribute
    {
        public int position;

        public SerializedAttribute(int position)
        {
            this.position = position;
        }


    }


}
