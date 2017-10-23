using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Util;
using DiscordMMO.Datatypes.Inventories;

namespace DiscordMMO.Datatypes.Items
{
    public abstract class ItemRangedWeapon : ItemWeapon
    {
        public abstract Item[] ammoTypes { get; }

        public override bool CanAttack(ref OnAttackEventArgs args)
        {
            // If the attacker is not a player, ignore everything
            if (args.attacker is Player == false)
                return true;

            Player player = args.attacker as Player;

            // Check if the player has a valid ammo type equipped
            if (ammoTypes.Where(x => x == player.equipment[PlayerEquipmentSlot.AMMO].itemType).Count() > 0)
            {
                lock (NumberUtil.randomLock)
                {
                    // Decide if the player will lose ammo
                    if (NumberUtil.random.NextDouble()*100 >= player.ammoLossChance)
                    {
                        player.equipment[PlayerEquipmentSlot.AMMO].count--;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
