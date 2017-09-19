using DiscordMMO.Handlers;
using DiscordMMO.Datatypes.Items;
using ProtoBuf;

namespace DiscordMMO.Datatypes
{
    [ProtoContract]
    public class ItemStack
    {
        [ProtoMember(1)]
        public Item itemType;

        [ProtoMember(2)]
        public int count;

        public bool IsEmpty => (itemType is ItemEmpty || count <= 0);

        public static ItemStack empty { get; private set; }

        public ItemStack()
        {
            itemType = ItemHandler.GetItemInstanceFromName("empty");
        }

        public ItemStack(Item type, int count)
        {
            itemType = type;
            this.count = count;
        }

        public ItemStack(Item type) : this(type, 1)
        {

        }

        static ItemStack()
        {
            if (empty == null)
            {
                empty = new ItemStack(ItemHandler.GetItemInstanceFromName("empty"));
            }
        }

        public string ToStringNoCount()
        {
            if (IsEmpty)
                return "Empty";
            return itemType.displayName;
        }
    }
}
