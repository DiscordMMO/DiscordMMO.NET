using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using DiscordMMO.Handlers;
using DiscordMMO.Datatypes.Messages;

namespace DiscordMMO.Util
{
    public static class ChannelUtil
    {

        public static Message SendMessage(this IMessageChannel channel, string msg, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            return SendMessage<MessageDefault>(channel, msg, isTTS, embed, options);
        }

        public static T SendMessage<T>(this IMessageChannel channel, string msg, bool isTTS = false, Embed embed = null, RequestOptions options = null) where T : Message, new()
        {
            return MessageHandler.SendMessageAsync<T>(channel, msg, isTTS, embed, options).GetAwaiter().GetResult();
        }
    }
}
