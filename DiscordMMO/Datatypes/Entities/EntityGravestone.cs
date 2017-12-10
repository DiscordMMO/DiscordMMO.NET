using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Interactions;

namespace DiscordMMO.Datatypes.Entities
{
    public class EntityGravestone : Entity
    {
        public override string name { get; set; } = "gravestone";
        public override string displayName { get; set; } = "\'s gravestone";

        public string playerName { get; set; }
        public List<ItemStack> items { get; set; }

        public EntityGravestone() : base() { }

        public EntityGravestone(Player player) : this(player.playerName, player.drops) { }

        public EntityGravestone(string playerName, List<ItemStack> items)
        {
            this.playerName = playerName;
            displayName = $"{playerName}\'s gravestone";

            // Clone the list, as lists are reference types, so when the items are cleared afterwards, they aren't cleared from the gravestone
            this.items = new List<ItemStack>(items);
            interactions.Add(new InteractionLookLoot { targetName = displayName, items = this.items });
            interactions.Add(new InteractionLoot { items = this.items });
        }

        /// <summary>
        /// Calling this function will set the display name to <code>{playername}'s gravestone</code>\n
        /// and add the interactions
        /// </summary>
        public void FieldsInitialized()
        {
            displayName = $"{playerName}\'s gravestone";
            interactions.Add(new InteractionLookLoot { targetName = displayName, items = items });
            interactions.Add(new InteractionLoot { items = items });
        }

    }
}
