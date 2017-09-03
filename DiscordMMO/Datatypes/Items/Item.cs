using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Util;
using DiscordMMO.Handlers;

namespace DiscordMMO.Datatypes.Items
{
    [SerializedClass("item")]
    public abstract class Item : ISerialized
    {

        //TODO: Possibly make this code cleaner?

        /// <summary>
        /// This is used as an id for the item
        /// This should be all lowercase with underscores for spaces
        /// </summary>
        [Serialized(0)]
        [DontInit]
        public virtual string itemName => "empty";

        public virtual string displayName => "Empty";

        public virtual bool stackable => true;

        public override string ToString()
        {
            return this.Serialize();
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

        [InstanceMethod(0)]
        public static Item CreateInstance(string name)
        {
            return ItemHandler.GetItemInstanceFromName(name);
        }

    }

    public class ItemEmpty : Item
    {
        public override string itemName => "empty";
        public override string displayName => "Empty";
    }

}
