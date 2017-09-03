using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DiscordMMO.Util;
using System.Runtime.Serialization;

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
                toAdd.Add(RegisterISerialized(item));
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
                if (type.GetCustomAttributes(typeof(SerializedClassAttribute), true).Length <= 0)
                    throw new ArgumentException($"Type {type.FullName} has not SerializedClass attribute");
                if (type.GetProperty("name") == null)
                {

                }

                SerializedClassAttribute classAttribute = type.GetCustomAttribute(typeof(SerializedClassAttribute)) as SerializedClassAttribute;

                if (classAttribute == null)
                {
                    throw new ArgumentException($"Type {type.FullName} has not SerializedClass attribute");
                }

                string name = classAttribute.prefix;

                if (IsRegisteredISerialized(name))
                    return;

                Dictionary<int, object> serializedProperties = new Dictionary<int, object>();

                foreach (FieldInfo f in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                {
                    foreach (object attribute in f.GetCustomAttributes(true))
                    {
                        SerializedAttribute sa = attribute as SerializedAttribute;
                        if (sa == null)
                            continue;
                        int position = sa.position;
                        serializedProperties[position] = f;
                        break;
                    }
                }

                foreach (PropertyInfo p in type.GetProperties())
                {
                    foreach (object attribute in p.GetCustomAttributes(true))
                    {
                        SerializedAttribute sa = attribute as SerializedAttribute;
                        if (sa == null)
                            continue;
                        int position = sa.position;
                        serializedProperties[position] = p;
                        break;
                    }
                }

                RegisteredSerialized registered = new RegisteredSerialized() { type = type, serializedVars = serializedProperties };


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

                string[] param = Regex.Match(s, "\\[(.*?)\\]").Value.Split(',');

                for (int i = 0; i < param.Length; i++)
                {
                    string el = param[i];
                    param[i] = el.Replace("]", "").Replace("[", "");
                }

                if (param.Length != ser.serializedVars.Keys.Count)
                {
                    throw new ArgumentException($"Argument length not matching for string \"{s}\"\n" +
                        $"Expected: {ser.serializedVars.Keys.Count}. Got: {param.Length}");
                }

                ISerialized ret = null;

                foreach (MethodInfo mi in ser.type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (mi.GetCustomAttributes(typeof(InstanceMethodAttribute), true).Count() <= 0)
                        continue;
                    InstanceMethodAttribute attribute = mi.GetCustomAttribute(typeof(InstanceMethodAttribute), true) as InstanceMethodAttribute;
                    if (attribute == null)
                        continue;
                    if (attribute.instanceIdentifierIndex.Length <= 0)
                    {
                        ret = (ISerialized)mi.Invoke(null, null);
                    }
                    else
                    {
                        List<object> initParam = new List<object>();
                        for (int i = 0; i < attribute.instanceIdentifierIndex.Length; i++)
                        {
                            if (ser.serializedVars.ContainsKey(attribute.instanceIdentifierIndex[i]))
                            {
                                initParam.Add(param[i]);
                            }
                            else
                            {
                                throw new ArgumentException($"Serializeable {ser.type.FullName} does not have a paramater with index {attribute.instanceIdentifierIndex} (InstanceMethodAttribute)");
                            }
                        }
                        ret = (ISerialized)mi.Invoke(null, initParam.ToArray());
                    
                    }
                    break;
                }

                if (ret == null)
                {
                    throw new ArgumentException($"Serializeable {ser.type.FullName} has no instance method");
                }

                for (int i = 0; i < param.Length; i++)
                {
                    if (ser.serializedVars[i] is FieldInfo)
                    {
                        FieldInfo fi = ser.serializedVars[i] as FieldInfo;
                        if (!ser.initializedVars.ContainsKey(i))
                        {
                            fi.SetValue(ret, BreakDown(param[i].ToString()));
                        }
                    }
                    else if (ser.serializedVars[i] is PropertyInfo)
                    {
                        PropertyInfo pi = ser.serializedVars[i] as PropertyInfo;
                        if (ser.initializedVars.ContainsKey(i))
                        {
                            if (pi.CanWrite)
                            {
                                pi.SetValue(ret, BreakDown(param[i].ToString()));
                            }
                            else
                            {
                                Console.WriteLine($"[Serialization Handler] WARNING: Property {pi.Module.FullyQualifiedName}.{pi.Name} has no setter, is marked as serializeable but not DontInit");
                            }
                        }
                    }
                }
                return ret;
            }
            throw new ArgumentException($"Could not deserialize string \"{s}\": Prefix \"{prefix}\" is not registered");
        }

        public static object BreakDown(string s)
        {
            if (int.TryParse(s, out int res))
            {
                return res;
            }
            try
            {
                ISerialized ret = Deserialize(s);
                return ret;
            }
            catch (ArgumentException e)
            {

            }
            return s;
        }

    }

    public struct RegisteredSerialized
    {
        public Type type;
        public Dictionary<int, object> serializedVars;

        public Dictionary<int, object> initializedVars
        {
            get
            {
                Dictionary<int, object> ret = new Dictionary<int, object>();
                foreach (int i in serializedVars.Keys)
                {
                    if (((MemberInfo)serializedVars[i]).GetCustomAttributes(typeof(DontInitAttribute), true).Length <= 0)
                    {
                        ret[i] = serializedVars[i];
                    }
                }
                return ret;
            }
        }


    }

}
