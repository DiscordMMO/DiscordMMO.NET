using System;
using System.Linq;
using System.Collections.Generic;
using DiscordMMO.Datatypes.Items;
using System.Reflection;

namespace DiscordMMO.Handlers
{
    public static class ItemHandler
    {
        private readonly static Dictionary<string, Type> items = new Dictionary<string, Type>();

        //TODO: Possibly clean this up?


        public static void Init()
        {
            var allItems = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                            from assemblyType in domainAssembly.GetTypes()
                            where typeof(Item).IsAssignableFrom(assemblyType) && !assemblyType.IsAbstract
                            select assemblyType).ToArray();
            foreach (Type item in allItems)
            {
                RegisterItem(item);
            }
        }

        public static Item GetItemInstanceFromName(string name, params object[] param)
        {
            return GetItemFromType(GetItemFromName(name), param);
        }

        public static Type GetItemFromName(string name)
        {
            return items[name];
        }

        public static Item GetItemFromType(Type item, params object[] param)
        {
            if (!item.IsSubclassOf(typeof(Item)))
            {
                throw new ArgumentException("Tried to get item instance from a type that is not an item");
            }
            return (Item)Activator.CreateInstance(item, param);
        }

        public static void RegisterItem(Type type)
        {
            if (!type.IsSubclassOf(typeof(Item)))
                throw new ArgumentException("Tried to register something that was not an item, as an item");
            if (type.GetProperty("name") == null)
            {

            }
            string name = (string) type.GetProperty("name").GetValue(null);
            Console.WriteLine($"[Item Handler] Registering item: {name}");
            items.Add(name, type);
        }

    }
}
