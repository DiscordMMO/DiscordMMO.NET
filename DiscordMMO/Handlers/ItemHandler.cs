using System;
using System.Linq;
using System.Collections.Generic;
using DiscordMMO.Datatypes.Items;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DiscordMMO.Handlers
{
    public static class ItemHandler
    {
        private readonly static Dictionary<string, Type> items = new Dictionary<string, Type>();

        //TODO: Possibly clean this up?

        
        public async static Task Init()
        {
            Console.WriteLine("[Item Handler] Detecting items");
            var watch = Stopwatch.StartNew();
            var allItems = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                            from assemblyType in domainAssembly.GetTypes()
                            where typeof(Item).IsAssignableFrom(assemblyType) && !assemblyType.IsAbstract
                            select assemblyType).ToArray();
            watch.Stop();
            Console.WriteLine("[Item Handler] Detecting items took " + watch.ElapsedMilliseconds + "ms");
            Console.WriteLine("[Item Handler] Average time per item: " + watch.ElapsedMilliseconds / allItems.Length + "ms");
            Console.WriteLine("[Item Handler] Registering items");
            watch = Stopwatch.StartNew();
            List<Task> toAdd = new List<Task>();
            foreach (Type item in allItems)
            {
                toAdd.Add(RegisterItem(item));
            }
            await Task.WhenAll(toAdd);
            watch.Stop();
            Console.WriteLine("[Item Handler] Registering items took " + watch.ElapsedMilliseconds + "ms");
        }

        public static Item GetItemInstanceFromName(string name, params object[] param)
        {
            if (GetItemFromName(name) == null)
            {

            }
            return GetItemFromType(GetItemFromName(name), param);
        }

        public static Type GetItemFromName(string name)
        {
            if (!items.ContainsKey(name))
                return null;
            return items[name];
        }

        public static Item GetItemFromType(Type item, params object[] param)
        {
            if (item == null)
            {

            }
            if (!item.IsSubclassOf(typeof(Item)))
            {
                throw new ArgumentException("Tried to get item instance from a type that is not an item");
            }
            return (Item)Activator.CreateInstance(item, param);
        }

        public static bool IsRegisteredItem(string name)
        {
            return items.ContainsKey(name);
        }

        public static async Task RegisterItem(Type type)
        {
            if (!type.IsSubclassOf(typeof(Item)))
                throw new ArgumentException("Tried to register something that was not an item, as an item");
            if (type.GetProperty("name") == null)
            {

            }
            Item item = GetItemFromType(type);
            string name = item.itemName;
            items.Add(name, type);
        }

        public static List<Type> GetItemTypes() => items.Values.ToList();

    }
}
