using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using DiscordMMO.Helpers;
using DiscordMMO.AreaLoaders;
using System.Reflection;

namespace DiscordMMO.Handlers
{
    public static class AreaLoaderHandler
    {

        private static List<Type> areaLoaders = new List<Type>();

        public async static Task Init()
        {
            Logger.Log("[Area Loader Handler] Detecting area loaders");
            var watch = Stopwatch.StartNew();
            var allItems = ReflectionHelper.GetTypesInheriting(Assembly.GetExecutingAssembly(), typeof(IAreaLoader));
            watch.Stop();
            Logger.Log("[Area Loader Handler] Detecting area loaders took " + watch.ElapsedMilliseconds + "ms");
            Logger.Log("[Area Loader Handler] Average time per area loader: " + watch.ElapsedMilliseconds / allItems.Count() + "ms");
            Logger.Log("[Area Loader Handler] Registering area loaders");
            watch = Stopwatch.StartNew();
            List<Task> toAdd = new List<Task>();
            foreach (Type item in allItems)
            {
                toAdd.Add(RegisterItem(item));
            }
            await Task.WhenAll(toAdd);
            watch.Stop();
            Logger.Log("[Area Loader Handler] Registering area loaders took " + watch.ElapsedMilliseconds + "ms");

            AreaHandler.beforeLoadEvent += PreLoad;
            AreaHandler.loadEvent += Load;
            AreaHandler.afterLoadEvent += PostLoad;

        }

        public static async Task RegisterItem(Type type)
        {

            if (!typeof(IAreaLoader).IsAssignableFrom(type))
            {
                throw new ArgumentException("Tried to register a non-area loader as an area loader. Class: " + type.AssemblyQualifiedName);
            }

            areaLoaders.Add(type);
        }

        private static void PreLoad()
        {
            foreach (Type t in areaLoaders)
            {
                IAreaLoader loader = (IAreaLoader)Activator.CreateInstance(t);
                loader.PreLoad();
            }
        }

        private static void Load()
        {
            foreach (Type t in areaLoaders)
            {
                IAreaLoader loader = (IAreaLoader)Activator.CreateInstance(t);
                loader.Load();
            }
        }

        private static void PostLoad()
        {
            foreach (Type t in areaLoaders)
            {
                IAreaLoader loader = (IAreaLoader)Activator.CreateInstance(t);
                loader.PostLoad();
            }
        }

    }

}
