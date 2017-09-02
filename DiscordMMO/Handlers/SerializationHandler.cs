using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DiscordMMO.Datatypes;

namespace DiscordMMO.Handlers
{
    public static class SerializationHandler
    {

        #region Registration

        private readonly static Dictionary<string, RegisteredSerialized> items = new Dictionary<string, RegisteredSerialized>();

        //TODO: Possibly clean this up?


        public async static Task Init()
        {
            Console.WriteLine("[Serialization Handler] Detecting serializeable objects");
            var watch = Stopwatch.StartNew();
            var allItems = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                            from assemblyType in domainAssembly.GetTypes()
                            where typeof(ISerialized).IsAssignableFrom(assemblyType) && typeof(ISerialized) != assemblyType
                            select assemblyType).ToArray();
            watch.Stop();
            Console.WriteLine("[Serialization Handler] Detecting serializeable objects took " + watch.ElapsedMilliseconds + "ms");
            Console.WriteLine("[Serialization Handler] Average time per serializeable object: " + watch.ElapsedMilliseconds / allItems.Length + "ms");
            Console.WriteLine("[Serialization Handler] Registering serializeable objects");
            watch = Stopwatch.StartNew();
            List<Task> toAdd = new List<Task>();
            foreach (Type item in allItems)
            {

            }
            await Task.WhenAll(toAdd);
            watch.Stop();
            Console.WriteLine("[Serialization Handler] Registering serializeable objects took " + watch.ElapsedMilliseconds + "ms");
        }

        public static ISerialized GetISerializedInstanceFromName(string name, params object[] param)
        {
            if (GetISerializedFromName(name) == null)
            {

            }
            return GetISerializedFromType(GetISerializedFromName(name), param);
        }

        public static RegisteredSerialized GetRegisteredSerialized(string name)
        {
            if (!items.ContainsKey(name))
                throw new ArgumentException($"Prefix \"{name}\" is not registered ");
            return items[name];
        }

        public static Type GetISerializedFromName(string name)
        {
            if (!items.ContainsKey(name))
                return null;
            return items[name].type;
        }

        public static ISerialized GetISerializedFromType(Type item, params object[] param)
        {
            if (item == null)
            {

            }
            if (!typeof(ISerialized).IsAssignableFrom(item))
            {
                throw new ArgumentException("Tried to get item instance from a type that is not an item");
            }
            return (ISerialized)Activator.CreateInstance(item, param);
        }

        public static bool IsRegisteredISerialized(string name)
        {
            return items.ContainsKey(name);
        }

        public static async Task RegisterISerialized(Type type)
        {
            try
            {
                if (!typeof(ISerialized).IsAssignableFrom(type))
                    throw new ArgumentException("Tried to register something that was not an ISerialized, as an ISerialized");
                if (type.GetProperty("name") == null)
                {

                }
                ISerialized item = GetISerializedFromType(type);

                List<object> serializedProperties = new List<object>();

                foreach (FieldInfo f in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                {
                    Attribute attribute = Attribute.GetCustomAttributes(f, typeof(SerializedAttribute), true)[0];

                    if (attribute == null)
                        continue;

                    if (attribute is SerializedAttribute)
                    {
                        SerializedAttribute sa = attribute as SerializedAttribute;
                        serializedProperties[sa.position] = f;
                        break;
                    }
                }

                foreach (PropertyInfo p in type.GetProperties())
                {
                    foreach (CustomAttributeData attribute in p.GetCustomAttributes(typeof(SerializedAttribute), true))
                    {
                        int position = (int)attribute.ConstructorArguments[0].Value;
                        serializedProperties[position] = p;
                        break;
                    }
                }

                RegisteredSerialized registered = new RegisteredSerialized() { type = type, serializedVars = serializedProperties.ToArray() };

                string name = item.convertPrefix;
                items.Add(name, registered);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        #endregion

        public static ISerialized Deserialize(string s)
        {
            string prefix = s.Split(':')[0];
            if (IsRegisteredISerialized(prefix))
            {
                RegisteredSerialized ser = GetRegisteredSerialized(prefix);
                ISerialized ret = (ISerialized)Activator.CreateInstance(ser.type);

                string[] param = Regex.Match(s, "\\[(.*?)\\]").Value.Split(',');

                for (int i = 0; i < param.Length; i++)
                {
                    string el = param[i];
                    param[i] = el.Replace("]", "").Replace("[", "");
                }

                if (param.Length != ser.serializedVars.Length)
                {
                    throw new ArgumentException($"Argument length not matching for string \"{s}\"\n" +
                        $"Expected: {ser.serializedVars.Length}. Got: {param.Length}");
                }

                for (int i = 0; i < param.Length; i++)
                {
                    if (ser.serializedVars[i] is FieldInfo)
                    {
                        FieldInfo fi = ser.serializedVars[i] as FieldInfo;
                        fi.SetValue(ret, param[i]);
                    }
                    else if (ser.serializedVars[i] is PropertyInfo)
                    {
                        PropertyInfo pi = ser.serializedVars[i] as PropertyInfo;
                        pi.SetValue(ret, param[i]);
                    }
                }

            }
            throw new ArgumentException($"Could not deserialize string \"{s}\": Prefix \"{prefix}\" is not registered");
        }


    }

    public struct RegisteredSerialized
    {
        public Type type;
        public object[] serializedVars;
    }

}
