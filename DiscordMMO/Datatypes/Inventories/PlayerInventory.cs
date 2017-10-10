using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using DiscordMMO.Handlers;
using System.IO;

namespace DiscordMMO.Datatypes.Inventories
{
    [XmlRoot]
    [HasOwnSerializer]
    public class PlayerInventory : LimitedInventory
    {

        [XmlIgnore]
        public Player owner { get; protected set; }

        public override int size => 28;

        [XmlElement]
        public override List<ItemStack> items { get => base.items; protected set => base.items = value; }

        protected PlayerInventory() { }

        public PlayerInventory(Player owner)
        {
            this.owner = owner;
        }

        public PlayerInventory(SerializationInfo info, StreamingContext context)
        {
            items = (List<ItemStack>)info.GetValue("items", typeof(List<ItemStack>));
        }

        public static PlayerInventory Deserialize(Stream s)
        {
            return (PlayerInventory)SerializationHandler.GetSerializer<PlayerInventory>().Deserialize(s);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("items", items);
        }
    }

}
