using DiscordMMO.Util;
using DiscordMMO.Handlers;

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

        protected PlayerInventory(){}

        public static PlayerInventory FromString(Player owner, string inv)
        {
            PlayerInventory ret = new PlayerInventory(owner);
            for (int i = 0; i < inv.Split(';').Length-1; i++)
            {
                ret[i] = (ItemStack)SerializationHandler.Deserialize(inv.Split(';')[i]);
            }
            return ret;
        }

    }

}
