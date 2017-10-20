using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Inventories;

namespace DiscordMMO.Datatypes.Items
{
    public class ItemTestHelmet : ItemEquipable
    {
        public override string itemName => "test_helmet";

        public override string displayName => "Test Helmet";

        public override bool stackable => false;

        public override PlayerEquipmentSlot slot => PlayerEquipmentSlot.HEAD;
    }
}
