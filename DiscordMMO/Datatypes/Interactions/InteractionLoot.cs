using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordMMO.Util;

namespace DiscordMMO.Datatypes.Interactions
{
    public class InteractionLoot : Interaction
    {
        public override string name { get; set; } = "loot";
        public override string displayName { get; set; } = "Loot";

        public List<ItemStack> items = new List<ItemStack>();

        public override void Interact(ref Player interactor, ICommandContext Context)
        {
            StringBuilder b = new StringBuilder($"{Context.User.Username}: Looted:\n");
            foreach (ItemStack item in items)
            {
                if (item == null || item.IsEmpty)
                    continue;
                if (interactor.inventory.CanAdd(item))
                {
                    interactor.inventory.AddItem(item);
                    b.Append(item.ToStringDisplay() + "\n");
                }
                else
                {
                    Context.Channel.SendMessage($"{Context.User.Username}: You do not have enough inventory space to loot that");
                    break;
                }
            }
            Context.Channel.SendMessage(b.ToString());
        }
    }
}
