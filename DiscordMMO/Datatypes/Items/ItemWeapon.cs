using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.Items
{
    public abstract class ItemWeapon : Item
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
        /// THIS IS NOT A PERCENTAGE CHANCE
        /// </summary>
        public abstract int accuracy { get; }

    }
}
