using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Reflection;

namespace DiscordMMO.Handlers
{
    public static class SerializationHandler
    {

        static Type[] primitiveWrappers = new Type[] { typeof(Byte), typeof(SByte), typeof(Int32), typeof(UInt32), typeof(Int16), typeof(UInt16), typeof(Int64), typeof(UInt64), typeof(Single), typeof(Double), typeof(Char), typeof(Boolean), typeof(Object), typeof(String), typeof(Decimal)};

        private static List<Type> allItems;

        private static Dictionary<Type, XmlSerializer> serializers = new Dictionary<Type, XmlSerializer>();


        public async static Task Init()
        {
            Console.WriteLine("[Serialization Handler] Detecting items");
            var watch = Stopwatch.StartNew();

            allItems = GetTypesWithXmlRootAttribute(Assembly.GetExecutingAssembly()).ToList();

            watch.Stop();
            Console.WriteLine("[Serialization Handler] Detecting items took " + watch.ElapsedMilliseconds + "ms");
            Console.WriteLine("[Serialization Handler] Average time per item: " + watch.ElapsedMilliseconds / allItems.Count + "ms");
            Console.WriteLine("[Serialization Handler] Registering items");
            watch = Stopwatch.StartNew();
            List<Task> toAdd = new List<Task>();
            foreach (Type item in allItems)
            {
                toAdd.Add(RegisterItem(item));
            }
            await Task.WhenAll(toAdd);
            watch.Stop();
            Console.WriteLine("[Serialization Handler] Registering items took " + watch.ElapsedMilliseconds + "ms");
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
                List<Type> inherited = allItems.Where(t => type.IsAssignableFrom(t) && t != type).ToList();

                inherited.AddRange(GetRequiredTypes(type));

                serializers.Add(type, new XmlSerializer(type, inherited.ToArray()));
            }
        }

        public static List<Type> GetRequiredTypes(Type type)
        {

            // FATAL: Fix the stackoverflow

            if (type.IsPrimitive || type == typeof(String) || type == typeof(object))
            {
                return new List<Type>();
            }
            
            List<Type> required = new List<Type>();

            var properties = type.GetProperties().Where(prop => prop.IsDefined(typeof(XmlElementAttribute), true));

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
                    required.Add(field.FieldType);
                    required.AddRange(GetRequiredTypes(field.FieldType));
                }
            }

            List<Type> inheriting = allItems.Where(t => type.IsAssignableFrom(t)).ToList();

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
}
