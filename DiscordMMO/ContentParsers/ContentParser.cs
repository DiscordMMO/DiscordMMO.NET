using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Handlers;
using DiscordMMO.Datatypes.Items;

namespace DiscordMMO.ContentParsers
{
    public abstract class ContentParser
    {
        public abstract string contentTypeFolderName { get; protected set; }
        public abstract float sortOrder { get; protected set; }

        public virtual T Parse<T>(StreamReader stream) where T : IContentParseable
        {
            return (T)SerializationHandler.GetSerializer<T>().Deserialize(stream);
        }
        public virtual IEnumerable<T> ParseAll<T>() where T : IContentParseable
        {
            string contentFolder = Environment.GetEnvironmentVariable("DISCORDMMO_USERDATA") + Path.DirectorySeparatorChar + contentTypeFolderName;
            if (!Directory.Exists(contentFolder))
            {
                Directory.CreateDirectory(contentFolder);
                yield break;
            }

            foreach (string file in Directory.GetFiles(contentFolder))
            {
                using (StreamReader s = new StreamReader(Path.Combine(contentFolder, file)))
                {
                    yield return Parse<T>(s);
                }
            }
        }

    }

    public interface IContentParseable
    {

    }

    public abstract class ContentParser<T> : ContentParser where T : IContentParseable
    {

        public T Parse(StreamReader stream)
        {
            return (T)SerializationHandler.GetSerializer<T>().Deserialize(stream);
        }

        public IEnumerable<T> ParseAll()
        {
            return null;
        }

    }

    public class ItemParser : ContentParser<Item>
    {
        public override string contentTypeFolderName { get; protected set; } = "items";
        public override float sortOrder { get; protected set; } = 0;
    }

}
