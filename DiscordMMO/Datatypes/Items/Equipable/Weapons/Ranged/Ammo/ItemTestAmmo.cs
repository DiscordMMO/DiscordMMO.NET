using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Inventories;

namespace DiscordMMO.Datatypes.Items.Equipable.Weapons.Ranged.Ammo
{
    public class ItemTestAmmo : ItemEquipable
    {
        public override string itemName => "test_ammo";

        public override string displayName => "Test Ammo";

        public override PlayerEquipmentSlot slot => PlayerEquipmentSlot.AMMO;
    }
}
