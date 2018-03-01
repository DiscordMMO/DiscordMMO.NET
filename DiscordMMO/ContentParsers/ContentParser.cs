using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Handlers;

namespace DiscordMMO.ContentParsers
{
    public abstract class ContentParser
    {
    }


    public abstract class ContentParser<T> : ContentParser
    {

        public abstract string contentTypeFolderName { get; protected set; }

        public virtual T Parse(StreamReader stream)
        {
            return (T)SerializationHandler.GetSerializer<T>().Deserialize(stream);
        }

        public virtual IEnumerable<T> ParseAll()
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
                    yield return Parse(s);
                }
            }

        }

    }

}
