using System.Xml.Serialization;
using DiscordMMO.Handlers;
using DiscordMMO.Datatypes.Items;
using DiscordMMO.Datatypes.LootTables;

namespace DiscordMMO.Datatypes
{
    [XmlRoot]
    [HasOwnSerializer]
    public class ItemStack : ILootTableContent
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

        public ItemStack(Item type, int count=1)
        {
            itemType = type;
            this.count = count;
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

        public ItemStack GetDrop() => this;
    }
}
