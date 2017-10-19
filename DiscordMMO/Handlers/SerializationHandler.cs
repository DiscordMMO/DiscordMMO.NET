using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Reflection;
using DiscordMMO.Datatypes;

namespace DiscordMMO.Handlers
{
    public static class SerializationHandler
    {

        static Type[] primitiveWrappers = new Type[] { typeof(Byte), typeof(SByte), typeof(Int32), typeof(UInt32), typeof(Int16), typeof(UInt16), typeof(Int64), typeof(UInt64), typeof(Single), typeof(Double), typeof(Char), typeof(Boolean), typeof(Object), typeof(String), typeof(Decimal)};

        private static List<Type> allItems;

        private static Dictionary<Type, XmlSerializer> serializers = new Dictionary<Type, XmlSerializer>();


        public async static Task Init()
        {
            Console.WriteLine("[Serialization Handler] Detecting serializeable items");
            var watch = Stopwatch.StartNew();

            allItems = GetTypesWithXmlRootAttribute(Assembly.GetExecutingAssembly()).ToList();

            watch.Stop();
            Console.WriteLine("[Serialization Handler] Detecting serializeable items took " + watch.ElapsedMilliseconds + "ms");
            Console.WriteLine("[Serialization Handler] Average time per item: " + watch.ElapsedMilliseconds / allItems.Count + "ms");
            Console.WriteLine("[Serialization Handler] Registering serializeable items");
            watch = Stopwatch.StartNew();
            List<Task> toAdd = new List<Task>();
            foreach (Type item in allItems)
            {
                toAdd.Add(RegisterItem(item));
            }
            await Task.WhenAll(toAdd);
            watch.Stop();
            Console.WriteLine("[Serialization Handler] Registering serializeable items took " + watch.ElapsedMilliseconds + "ms");
            Console.WriteLine("[Serialization Handler] Average registration time per item: " + watch.ElapsedMilliseconds / allItems.Count + "ms");
        }

        public static IEnumerable<Type> GetTypesWithXmlRootAttribute(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(XmlRootAttribute), true).Length > 0)
                {
                    yield return type;
                }
            }
        }

        public static async Task RegisterItem(Type type)
        {
            if (type.GetCustomAttributes(typeof(HasOwnSerializerAttribute)).Count() > 0)
            {
                List<Type> required = allItems.Where(t => type.IsAssignableFrom(t) && t != type).ToList();

                required.AddRange(GetRequiredTypes(type));

                serializers.Add(type, new XmlSerializer(type, required.ToArray()));
            }
        }

        public static List<Type> GetRequiredTypes(Type type)
        {

            if (type == typeof(Action))
            {

            }

            if (type.IsPrimitive || primitiveWrappers.Contains(type))
            {
                return new List<Type>();
            }
            
            List<Type> required = new List<Type>();

            var properties = type.GetProperties().Where(prop => prop.IsDefined(typeof(XmlElementAttribute)) && !prop.IsDefined(typeof(XmlIgnoreAttribute), true));

            foreach (var property in properties)
            {
                if (!required.Contains(property.PropertyType))
                {
                    if (property.PropertyType.IsPrimitive || primitiveWrappers.Contains(property.PropertyType))
                        continue;
                    required.Add(property.PropertyType);
                    required.AddRange(GetRequiredTypes(property.PropertyType));
                }
            }

            var fields = type.GetFields().Where(field => field.IsDefined(typeof(XmlElementAttribute), true));

            foreach (var field in fields)
            {
                if (!required.Contains(field.FieldType))
                {
                    if (field.FieldType.IsPrimitive || primitiveWrappers.Contains(field.FieldType))
                        continue;
                    required.Add(field.FieldType);
                    required.AddRange(GetRequiredTypes(field.FieldType));
                }
            }

            var alsoRequired = type.GetCustomAttributes<AlsoRequiresAttribute>().Select(t => t.required);

            foreach(Type req in alsoRequired)
            {
                required.Add(req);
                required.AddRange(GetRequiredTypes(req));
            }

            List<Type> inheriting = Assembly.GetAssembly(type).ExportedTypes.Where(t => t.IsSubclassOf(type) && type != t).ToList();

            foreach (Type t in inheriting)
            {
                required.AddRange(GetRequiredTypes(t));
            }

            required.AddRange(inheriting);

            return required;
        }

        public static XmlSerializer GetSerializer<T>()
        {
            if (serializers.ContainsKey(typeof(T)))
                return serializers[typeof(T)];
            return serializers[GetElementWithOwnSerializer(typeof(T))];
        }

        public static Type GetElementWithOwnSerializer(Type t)
        {
            if (t.GetCustomAttributes(typeof(HasOwnSerializerAttribute)).Count() > 0)
                return t;
            if (t.BaseType == null || t.BaseType == typeof(object))
                return null;

            return GetElementWithOwnSerializer(t.BaseType);
        }

    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class HasOwnSerializerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class AlsoRequiresAttribute : Attribute
    {

        public Type required;

        public AlsoRequiresAttribute(Type required)
        {
            this.required = required;
        }

    }


}
