using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Handlers;

namespace DiscordMMO.Datatypes.Items
{
    public class ItemTestBow : ItemRangedWeapon
    {
        public override Item[] ammoTypes { get => new Item[] { ItemHandler.GetItemInstanceFromName("test_ammo") }; }

        public override int attackRate => 2;

        public override int attackDamage => 3;

        public override int accuracy => 90;

        public override string itemName => "test_bow";

        public override string displayName => "Test Bow";
    }
}
