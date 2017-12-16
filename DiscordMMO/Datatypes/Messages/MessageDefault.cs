using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Handlers;

namespace DiscordMMO.Datatypes.Messages
{
    public class MessageDefault : Message
    {
        public override DateTime expiry { get; set; } = DateTime.Now.AddSeconds(MessageHandler.DEFAULT_MESSAGE_LIFESPAN);
    }
}
