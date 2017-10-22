using System;
using DiscordMMO.Handlers;
using System.Xml.Serialization;

namespace DiscordMMO.Datatypes.Items
{
    [XmlRoot]
    [HasOwnSerializer]
    public abstract class Item
    {

        //TODO: Possibly make this code cleaner?

        /// <summary>
        /// This is used as an id for the item
        /// This should be all lowercase with underscores for spaces
        /// </summary>
        [XmlElement]
        public virtual string itemName => "empty";

        [XmlIgnore]
        public virtual string displayName => "Empty";

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

    }

    public class ItemEmpty : Item
    {
        public override string itemName => "empty";
        public override string displayName => "Empty";
    }

}
