using System;
using DiscordMMO.Handlers;
using System.Xml.Serialization;
using DiscordMMO.ContentParsers;

namespace DiscordMMO.Datatypes.Items
{
    [XmlRoot]
    [HasOwnSerializer]
    public abstract class Item : IContentParseable
    {


        /// <summary>
        /// This is used as an id for the item
        /// This should be all lowercase with underscores for spaces
        /// </summary>
        [XmlElement]
        public abstract string itemName { get; }

        [XmlIgnore]
        public abstract string displayName { get; }

        [XmlElement]
        public virtual bool stackable => true;

        public override string ToString()
        {
            return itemName;
        }

        public static implicit operator ItemStack(Item item)
        {
            return new ItemStack(item);
        }

        public static explicit operator Item(Type type)
        {
            if (!type.IsAssignableFrom(typeof(Item)))
            {
                throw new ArgumentException($"Attempted to cast a non-item type ({type.FullName}) to an item");
            }
            return ItemHandler.GetItemFromType(type);
        }

        public static bool operator ==(Item item1, Item item2) => item1.itemName == item2.itemName;
        public static bool operator !=(Item item1, Item item2) => item1.itemName != item2.itemName;

        public override bool Equals(object obj)
        {
            if (obj is Item == false)
                return false;
            return this == (obj as Item);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

    public class ItemEmpty : Item
    {
        public override string itemName => "empty";
        public override string displayName => "Empty";
    }

}
