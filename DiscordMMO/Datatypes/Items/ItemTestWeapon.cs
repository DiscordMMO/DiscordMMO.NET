using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.Items
{
    public class ItemTestWeapon : ItemWeapon
    {
        public override string itemName => "test_weapon";

        public override string displayName => "Test Weapon";

        public override int attackRate => 6;

        public override int attackDamage => 5;

        public override int accuracy => 75;
    }
}
