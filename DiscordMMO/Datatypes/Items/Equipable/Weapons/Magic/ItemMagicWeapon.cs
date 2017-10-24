using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Util;

namespace DiscordMMO.Datatypes.Items.Equipable.Weapons.Magic
{
    public abstract class ItemMagicWeapon : ItemWeapon
    {

        public abstract int manaUsage { get; }

        public override bool CanAttack(ref OnAttackEventArgs args)
        {
            base.CanAttack(ref args);
            if (args.attacker is Player == false)
                return true;

            Player player = args.attacker as Player;

            if (manaUsage*player.manaUsageModifier > player.mana)
                return false;

            player.mana -= (int)Math.Floor(manaUsage * player.manaUsageModifier);
            return true;

        }

    }
}
