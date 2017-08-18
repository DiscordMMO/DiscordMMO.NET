using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.Inventories
{
    public class PlayerInventory : LimitedInventory
    {
        public Player owner { get; protected set; }

        public override int size => 28;

        public PlayerInventory(Player owner)
        {
            this.owner = owner;
        }

        public static PlayerInventory FromString(Player owner, string inv)
        {
            PlayerInventory ret = new PlayerInventory(owner);
            for (int i = 0; i < inv.Split(';').Length; i++)
            {
                ret[i] = ItemStack.FromString(inv.Split(';')[i]);
            }
            return ret;
        }

    }

}
