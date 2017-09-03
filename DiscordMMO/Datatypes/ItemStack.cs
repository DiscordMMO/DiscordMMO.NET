using System;
using System.Collections.Generic;
using DiscordMMO.Util;
using DiscordMMO.Handlers;
using DiscordMMO.Datatypes.Items;

namespace DiscordMMO.Datatypes
{
    [SerializedClass("itemstack")]
    public class ItemStack : ISerialized
    {
        [Serialized(0)]
        [DontInit]
        public Item itemType;

        [Serialized(1)]
        public int count;

        public bool IsEmpty => (itemType is ItemEmpty || count <= 0);

        public static ItemStack empty { get; private set; }


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

        public override string ToString()
        {
            return Serialize();
        }

        public string Serialize()
        {
            if (itemType == null)
            {
                itemType = ItemHandler.GetItemInstanceFromName("empty");
            }
            return SerializationExtension.Serialize(this);
        }

        public string ToStringNoCount()
        {
            if (IsEmpty)
                return "Empty";
            return itemType.displayName;
        }

        [InstanceMethod(0)]
        public static ItemStack CreateInstance(Item item)
        {
            return new ItemStack(item);
        }


    }
}
