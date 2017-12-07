using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordMMO.Util;

namespace DiscordMMO.Datatypes.Interactions
{
    public class InteractionLookLoot : Interaction
    {
        public override string name { get; set; } = "lookLoot";
        public override string displayName { get; set; } = "View loot";

        public string targetName { get; set; }

        public List<ItemStack> items;

        public override void Interact(ref Player interactor, ICommandContext Context)
        {
            StringBuilder builder = new StringBuilder($"{Context.User.Username}: You look in the {targetName} and see:\n");
            int i = 1;
            foreach (ItemStack item in items)
            {
                builder.Append($"{i}: {item.ToStringDisplay()}\n");
                i++;
            }
            Context.Channel.SendMessage(builder.ToString());
        }
    }
}
