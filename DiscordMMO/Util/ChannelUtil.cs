using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DiscordMMO.Util
{
    public static class ChannelUtil
    {

        public static void SendMessage(this IMessageChannel channel, string msg)
        {
            channel.SendMessageAsync(msg).GetAwaiter().GetResult();
        }

    }
}
