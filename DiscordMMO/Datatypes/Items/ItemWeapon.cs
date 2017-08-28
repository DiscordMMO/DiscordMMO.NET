using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Inventories;

namespace DiscordMMO.Datatypes.Items
{
    public abstract class ItemWeapon : ItemEquipable
    {

        /// <summary>
        /// The ticks between each attack
        /// </summary>
        public abstract int attackRate { get; }

        /// <summary>
        /// The base max hit of this weapon
        /// </summary>
        public abstract int attackDamage { get; }

        /// <summary>
        /// The chance that this weapon hits
        /// </summary>
        public abstract int accuracy { get; }

        public override bool stackable => false;

        public override PlayerEquipmentSlot slot => PlayerEquipmentSlot.MAIN_HAND;
    }
}
