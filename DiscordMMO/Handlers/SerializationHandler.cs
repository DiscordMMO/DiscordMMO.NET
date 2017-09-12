using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DiscordMMO.Util;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DiscordMMO.Handlers
{
    public static class SerializationHandler
    {

        public static readonly Regex deserializationRegex = new Regex("^[^\\[\\[]*" +
                       "(" +
                       "((?'Open'\\[)[^\\[\\]]*)+" +
                       "((?'Close-Open'\\])[^\\[\\]]*)+" +
                       ")*" +
                       "(?(Open)(?!))$", RegexOptions.IgnorePatternWhitespace);

        #region Registration

        private readonly static List<Type> items = new List<Type>();

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

        public static byte[] Serialize(ISerializable ser)
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, ser);
            }
            byte[] ret = stream.ToArray();
            return ret;
        }

        public static async Task RegisterISerialized(Type type)
        {

            if (!typeof(ISerialized).IsAssignableFrom(type))
                throw new ArgumentException("Tried to register something that was not an ISerialized, as an ISerialized");

            items.Add(type);

            #region Deprecated
                /*
                try
                {
                    if (!typeof(ISerialized).IsAssignableFrom(type))
                        throw new ArgumentException("Tried to register something that was not an ISerialized, as an ISerialized");
                    if (type.GetCustomAttributes(typeof(SerializedClassAttribute), true).Length <= 0)
                        throw new ArgumentException($"Type {type.FullName} has no SerializedClass attribute");

                    SerializedClassAttribute classAttribute = type.GetCustomAttribute(typeof(SerializedClassAttribute)) as SerializedClassAttribute;

                    if (classAttribute == null)
                    {
                        throw new ArgumentException($"Type {type.FullName} has no SerializedClass attribute");
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

                    RegisteredSerialized registered = new RegisteredSerialized() { type = type, serializedVars = serializedProperties, prefix = name };


                    items.Add(name, registered);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                */
                #endregion
        }

        #endregion



        public static ISerialized Deserialize(string s, params object[] extraParams)
        {
            return null;
            #region Deprecated
            /*
            string prefix = s.Split(':')[0];
            if (IsRegisteredISerialized(prefix))
            {
                RegisteredSerialized ser = GetRegisteredSerialized(prefix);

                // TODO: Figure out nested serialization

                List<string> param = Nested(s).ToList();
                
                for (int i = 0; i < param.Count; i++)
                {
                    string el = param[i];
                    param[i] = el.Replace("]", "").Replace("[", "");
                }

                if (param.Count != ser.serializedVars.Keys.Count)
                {
                    throw new ArgumentException($"Argument length not matching for string \"{s}\"\n" +
                        $"Expected: {ser.serializedVars.Keys.Count}. Got: {param.Count}");
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
                        ret = (ISerialized)mi.Invoke(null, extraParams);
                    }
                    else
                    {
                        List<object> initParam = extraParams.ToList();
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

                for (int i = 0; i < param.Count; i++)
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
            */
#endregion
        }

        private static IEnumerable<String> Nested(string value)
        {
            if (string.IsNullOrEmpty(value))
                yield break; // or throw exception

            Stack<int> brackets = new Stack<int>();

            for (int i = 0; i < value.Length; ++i)
            {
                char ch = value[i];

                if (ch == '[')
                    brackets.Push(i);
                else if (ch == ']')
                {
                    //TODO: you may want to check if close ']' has corresponding open '['
                    // i.e. stack has values: if (!brackets.Any()) throw ...
                    int openBracket = brackets.Pop();

                    yield return value.Substring(openBracket - 1, i - openBracket);
                }
            }

            //TODO: you may want to check here if there are too many '['
            // i.e. stack still has values: if (brackets.Any()) throw ... 

            yield return value;
        }

        public static object BreakDown(object s)
        {

            if (s is string)
            {

                if (int.TryParse(s.ToString(), out int res))
                {
                    return res;
                }
                try
                {
                    ISerialized ret = Deserialize(s.ToString());
                    return ret;
                }
                catch (ArgumentException e)
                {

                }
                return s;
            }
            return s;
        }

    }
}
