using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DiscordMMO.Datatypes.Messages
{
    // TODO: Come up with a better name
    public class MessageStaying : Message
    {

        public MessageStaying() : base()
        {
            expiry = DateTime.MaxValue.AddYears(-1);
        }

        public MessageStaying(IMessage msg) : this()
        {
            message = msg;
        }

    }
}
