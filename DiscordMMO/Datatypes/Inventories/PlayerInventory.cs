using DiscordMMO.Handlers;
using ProtoBuf;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DiscordMMO.Datatypes.Inventories
{
    [ProtoContract, ProtoInclude(0, typeof(PlayerEquimentInventory))]
    public class PlayerInventory : LimitedInventory
    {

        public Player owner { get; protected set; }

        public override int size => 28;

        [ProtoMember(0)]
        public override List<ItemStack> items { get => base.items; protected set => base.items = value; }

        public PlayerInventory(Player owner)
        {
            this.owner = owner;
        }

        public PlayerInventory(SerializationInfo info, StreamingContext context)
        {
            items = (List<ItemStack>)info.GetValue("items", typeof(List<ItemStack>));
        }

        public static PlayerInventory FromString(Player owner, string inv)
        {
            PlayerInventory ret = new PlayerInventory(owner);
            for (int i = 0; i < inv.Split(';').Length-1; i++)
            {
                ret[i] = (ItemStack)SerializationHandler.Deserialize(inv.Split(';')[i]);
            }
            return ret;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("items", items);
        }
    }

}
