using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DiscordMMO.Datatypes.Messages
{
    public class MessagePermanent : Message
    {

        public override DateTime expiry { get; set; } = DateTime.MaxValue.AddYears(-1);

    }
}
