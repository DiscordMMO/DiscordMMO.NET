using System.Xml.Serialization;
using DiscordMMO.Handlers;
using DiscordMMO.Datatypes.Items;

namespace DiscordMMO.Datatypes
{
    [XmlRoot]
    [HasOwnSerializer]
    public class ItemStack
    {
        [XmlElement]
        public Item itemType;

        [XmlElement]
        protected int _count;

        public int count
        {
            get => _count;

            set
            {
                if (value <= 0)
                {
                    _count = 1;
                    itemType = empty.itemType;
                    return;
                }
                else
                {
                    _count = value;
                }
            }
        }


        [XmlIgnore]
        public bool IsEmpty => (itemType is ItemEmpty || count <= 0);

        [XmlIgnore]
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

        public string ToStringDisplay()
        {
            if (IsEmpty)
                return "Empty";

            return itemType.displayName + (count == 1 ? "" : $" ({count})");
        }

        public override string ToString()
        {
            string afterItemName = count <= 0 ? "" : $", {count}";
            return $"({itemType.itemName})";
        }

    }
}
