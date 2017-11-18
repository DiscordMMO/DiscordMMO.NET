using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DiscordMMO.Datatypes.Interactions.Dialogues
{
    public class DialogueNodeGiveItems : DialogueNode
    {

        public List<ItemStack> items = new List<ItemStack>();

        public override void Execute(ref Player player, IMessageChannel channel)
        {
            base.Execute(ref player, channel);
            foreach (ItemStack stack in items)
            {
                player.inventory.AddItem(stack);
            }
        }

    }
}
