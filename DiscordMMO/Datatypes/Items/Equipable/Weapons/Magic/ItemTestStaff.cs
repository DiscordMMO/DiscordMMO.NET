using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.Items.Equipable.Weapons.Magic
{
    public class ItemTestStaff : ItemMagicWeapon
    {
        public override int manaUsage => 60;

        public override int attackRate => 5;

        public override int attackDamage => 5;

        public override int accuracy => 75;

        public override string itemName => "test_staff";

        public override string displayName => "Test Staff";
    }
}
