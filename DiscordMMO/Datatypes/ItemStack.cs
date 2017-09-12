using System;
using System.Runtime.Serialization;
using DiscordMMO.Util;
using DiscordMMO.Handlers;
using DiscordMMO.Datatypes.Items;

namespace DiscordMMO.Datatypes
{
    [Serializable]
    public class ItemStack : ISerializable
    {
        public Item itemType;

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

        public ItemStack(SerializationInfo info, StreamingContext context)
        {
            itemType = ItemHandler.GetItemInstanceFromName(info.GetString("itemType"));
            count = info.GetInt32("count");
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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("type", itemType.itemName);
            info.AddValue("count", count);
        }
    }
}
