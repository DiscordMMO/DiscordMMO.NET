using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.Items.ItemEffects
{
    public abstract class ItemEffect
    {

        public readonly string name;
        public string displayName;

        public string description;

        protected ItemEffect(string name, string displayName, string description)
        {
            this.name = name;
            this.displayName = displayName;
            this.description = description;
        }

        public abstract void OnEquipped(Player player);
        public abstract void WhileEquipped(Player player);
        public abstract void OnUnequip(Player player);

    }
}
