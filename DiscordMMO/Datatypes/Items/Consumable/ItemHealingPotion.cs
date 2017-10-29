using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Entities;

namespace DiscordMMO.Datatypes.Items.Consumable
{
    public class ItemHealingPotion : ItemConsumable
    {

        public readonly int healAmount = 10;

        public override string itemName => "healing_potion";

        public override string displayName => "Healing Potion";

        public override bool stackable => false;

        public override void Consume(Player player)
        {
            player.Heal(healAmount);
        }
    }
}
