using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.Items.Consumable
{
    public abstract class ItemConsumable : Item
    {
        public abstract void Consume(Player player);
    }
}
