using System;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DiscordMMO.Util;
using DiscordMMO.Handlers;

namespace DiscordMMO.Datatypes.Items
{
    [Serializable]
    public abstract class Item : ISerializable
    {

        //TODO: Possibly make this code cleaner?

        /// <summary>
        /// This is used as an id for the item
        /// This should be all lowercase with underscores for spaces
        /// </summary>
        public virtual string itemName => "empty";

        public virtual string displayName => "Empty";

        public virtual bool stackable => true;


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("itemName", itemName);
        }


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

    }

    public class ItemEmpty : Item
    {
        public override string itemName => "empty";
        public override string displayName => "Empty";
    }

}
