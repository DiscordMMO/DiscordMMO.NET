using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Util
{

    public interface ISerialized
    {
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

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class SerializedClassAttribute : Attribute
    {
        public string prefix;

        public SerializedClassAttribute(string prefix)
        {
            this.prefix = prefix;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class InstanceMethodAttribute : Attribute
    {
        public int[] instanceIdentifierIndex;

        public InstanceMethodAttribute(params int[] instanceIdentifierIndex)
        {
            this.instanceIdentifierIndex = instanceIdentifierIndex;
        }

        public InstanceMethodAttribute(int instanceIdentifierIndex)
        {
            this.instanceIdentifierIndex = new int[] { instanceIdentifierIndex };
        }

        public InstanceMethodAttribute()
        {
            instanceIdentifierIndex = new int[0];
        }
    }

    /// <summary>
    /// Members with this attribute will not be initialized in deserialization
    /// </summary>
    [AttributeUsage((AttributeTargets.Field | AttributeTargets.Property), AllowMultiple = false, Inherited = true)]
    public class DontInitAttribute : Attribute
    {

    }


}
