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

        public PlayerInventory(Player owner) : base(true)
        {
            this.owner = owner;
        }

    }

}
