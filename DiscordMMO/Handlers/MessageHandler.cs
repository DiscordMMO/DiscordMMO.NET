using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes;
using Discord;

namespace DiscordMMO.Handlers
{
    public static class MessageHandler
    {

        /// <summary>
        /// The amount of seconds that can pass before a message is deleted
        /// </summary>
        public const int DEFAULT_MESSAGE_LIFESPAN = 120;

        private static List<Message> messages = new List<Message>();

        public static async Task Tick()
        {

            List<Task> toDelete = new List<Task>();

            foreach (Message msg in messages)
            {
                if (msg.IsExpired)
                {
                    toDelete.Add(msg.DeleteAsync());
                }
            }

            await Task.WhenAll(toDelete);
        }

        public static async Task<Message> SendMessageAsync(IMessageChannel channel, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            return await SendMessageAsync<Message>(channel, text, isTTS, embed, options);
        }

        public static async Task<T> SendMessageAsync<T>(IMessageChannel channel, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null) where T : Message, new()
        {
            IMessage msg = await channel.SendMessageAsync(text, isTTS, embed, options) as IMessage;
            T sent = new T { message = msg };
            messages.Add(sent);
            return sent;
        }


        public static void RemoveMessage(Message toRemove)
        {
            messages.Remove(toRemove);
        }

    }
}
