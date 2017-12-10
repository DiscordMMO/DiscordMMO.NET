using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using DiscordMMO.Handlers;

namespace DiscordMMO.Datatypes
{
    public class SentMessage
    {

        public IMessage message;

        public DateTime expiry;

        public virtual bool IsExpired => expiry <= DateTime.Now;

        public SentMessage()
        {

        }

        public SentMessage(IMessage msg)
        {
            message = msg;
            expiry = DateTime.Now.AddSeconds(MessageHandler.DEFAULT_MESSAGE_LIFESPAN);
        }

        public SentMessage(IMessage msg, DateTime expiryDate)
        {
            message = msg;
            expiry = expiryDate;
        }

        public async Task DeleteAsync()
        {
            await message.DeleteAsync();
            MessageHandler.RemoveMessage(this);
        }

        /// <summary>
        /// <see cref="DeleteAsync"/> is generally preffered, but if async is not an option, this works fine
        /// </summary>
        public void Delete()
        {
            DeleteAsync().GetAwaiter().GetResult();
        }

    }
}
