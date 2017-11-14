using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DiscordMMO.Helpers
{
    public static class ReflectionHelper
    {

        public static IEnumerable<Type> GetTypesWithAttribute(Assembly assembly, Type attributeType)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(attributeType, true).Length > 0)
                {
                    yield return type;
                }
            }
        }
        
        public static IEnumerable<Type> GetTypesInheriting(Assembly assembly, Type parent)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (parent.IsAssignableFrom(type) && !(type.IsAbstract || type.IsInterface))
                {
                    yield return type;
                }
            }
        }

    }
}
