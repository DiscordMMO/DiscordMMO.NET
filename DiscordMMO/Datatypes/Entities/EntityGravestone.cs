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

        public void FieldsInitialized()
        {
            displayName = $"{playerName}\'s gravestone";
            interactions.Add(new InteractionLookLoot { targetName = displayName, items = items });
            interactions.Add(new InteractionLoot { items = items });
        }

    }
}
