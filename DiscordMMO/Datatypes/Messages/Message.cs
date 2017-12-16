using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Handlers;
using Discord;

namespace DiscordMMO.Datatypes.Messages
{
    public abstract class Message
    {

        public IMessage message;

        public abstract DateTime expiry { get; set; }

        public virtual bool IsExpired => expiry <= DateTime.Now;

        public Message()
        {

        }

        public Message(IMessage msg) : this()
        {
            message = msg;
        }

        public Message(IMessage msg, DateTime expiryDate) : this(msg)
        {
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
