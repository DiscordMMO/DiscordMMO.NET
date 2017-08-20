using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Handlers;

namespace DiscordMMO.Datatypes.Items
{
    public abstract class Item
    {

        //TODO: Possibly make this code cleaner?

        /// <summary>
        /// This is used as an id for the item
        /// This should be all lowercase with underscores for spaces
        /// </summary>
        public static string name { get; }

        public virtual string itemName => name;

        public abstract string displayName { get; }

        public override string ToString()
        {
            return displayName;
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

    }

    public class ItemEmpty : Item
    {
        public static new string name => "empty";
        public override string displayName => "Empty";
    }

}
