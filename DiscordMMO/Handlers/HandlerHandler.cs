using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using DiscordMMO.Helpers;
using Discord;

namespace DiscordMMO.Handlers
{
    // M E T A
    // E
    // T
    // A
    public static class HandlerHandler
    {

        public async static Task Init()
        {
            Logger.Log("[Handler Handler] Detecting handlers");
            var watch = Stopwatch.StartNew();

            List<Type> allItems = ReflectionHelper.GetTypesWithAttribute(Assembly.GetExecutingAssembly(), typeof(HandlerAttribute)).ToList();

            watch.Stop();
            Logger.Log("[Handler Handler] Detecting handlers took " + watch.ElapsedMilliseconds + "ms");
            Logger.Log("[Handler Handler] Average time per item: " + watch.ElapsedMilliseconds / allItems.Count + "ms");
            List<Task> toAdd = new List<Task>();

            // Sort the handlers by priority
            allItems.Sort((i1, i2) => i1.GetCustomAttribute<HandlerAttribute>().priority.CompareTo(i2.GetCustomAttribute<HandlerAttribute>().priority));

            foreach (Type item in allItems)
            {
                foreach (MethodInfo m in item.GetMethods())
                {
                    if (m.GetCustomAttribute<InitMethodAttribute>() != null)
                    {
                        if (m.ReturnType.Equals(typeof(Task)))
                        {
                            toAdd.Add((Task)m.Invoke(null, null));
                        }
                        else
                        {
                            Logger.Log($"Method {m.Name} was marked as an Init Method but is not a task", LogSeverity.Error);
                        }
                    }
                    else continue;
                }
            }

            Logger.Log($"[Handler Handler] Initializing {allItems.Count} handlers");

            watch = Stopwatch.StartNew();

            await Task.WhenAll(toAdd);

            watch.Stop();

            Logger.Log($"[Handler Handler] Initializing {allItems.Count} handlers took {watch.ElapsedMilliseconds}ms");
            Logger.Log($"[Handler Handler] Average time per handler: {watch.ElapsedMilliseconds/allItems.Count}ms");

        }

    }

    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false)]
    public class HandlerAttribute : Attribute
    {
        public float priority;

        public HandlerAttribute(float priority)
        {
            this.priority = priority;
        }
    }

    [AttributeUsage(validOn: AttributeTargets.Method, AllowMultiple = false)]
    public class InitMethodAttribute : Attribute
    {

    }

}
