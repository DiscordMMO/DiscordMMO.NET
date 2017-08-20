using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.Items
{
    public class ItemWood : Item
    {

        public static new string name => "wood";

        public override string itemName => name;

        public override string displayName => "Wood";

    }
}
