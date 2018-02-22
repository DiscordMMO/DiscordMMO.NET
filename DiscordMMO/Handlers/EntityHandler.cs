using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Entities;

namespace DiscordMMO.Handlers
{
    public static class EntityHandler
    {
        private static Dictionary<string, Type> entities = new Dictionary<string, Type>();

        public async static Task Init()
        {
            Logger.Log("[Entity Handler] Detecting entities");
            var watch = Stopwatch.StartNew();
            var allEntities = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                            from assemblyType in domainAssembly.GetTypes()
                            where typeof(Entity).IsAssignableFrom(assemblyType) && !assemblyType.IsAbstract
                            select assemblyType).ToArray();
            watch.Stop();
            Logger.Log("[Entity Handler] Detecting entities took " + watch.ElapsedMilliseconds + "ms");
            Logger.Log("[Entity Handler] Average time per entity: " + watch.ElapsedMilliseconds / allEntities.Length + "ms");
            Logger.Log("[Entity Handler] Registering entities");
            watch = Stopwatch.StartNew();
            List<Task> toAdd = new List<Task>();
            foreach (Type entity in allEntities)
            {
                toAdd.Add(RegisterEntity(entity));
            }
            await Task.WhenAll(toAdd);
            watch.Stop();
            Logger.Log("[Entity Handler] Registering entities took " + watch.ElapsedMilliseconds + "ms");
        }

        public static Entity GetEntityInstanceFromName(string name, params object[] param)
        {
            if (GetEntityFromName(name) == null)
            {

            }
            return GetEntityFromType(GetEntityFromName(name), param);
        }

        public static Type GetEntityFromName(string name)
        {
            if (!entities.ContainsKey(name))
                return null;
            return entities[name];
        }

        public static Entity GetEntityFromType(Type entity, params object[] param)
        {
            if (entity == null)
            {

            }
            if (!entity.IsSubclassOf(typeof(Entity)))
            {
                throw new ArgumentException("Tried to get entity instance from a type that is not an entity");
            }
            return (Entity)Activator.CreateInstance(entity, param);
        }

        public static bool IsRegisteredEntity(string name)
        {
            return entities.ContainsKey(name);
        }

        public static async Task RegisterEntity(Type type)
        {
            if (!type.IsSubclassOf(typeof(Entity)))
                throw new ArgumentException("Tried to register something that was not an entity, as an entity");
            if (type.GetProperty("name") == null)
            {

            }
            Entity entity = GetEntityFromType(type);
            string name = entity.name;
            entities.Add(name, type);
        }

    }
}
