using DiscordMMO.Datatypes.Inventories;
using DiscordMMO.Util;

namespace DiscordMMO.Datatypes.Items
{
    public abstract class ItemEquipable : Item
    {

        public abstract PlayerEquipmentSlot slot { get; }

        public virtual void OnEquip(Player player)
        {

        }

        public virtual void OnUnEquip(Player player)
        {

        }

        public virtual bool CanAttack(ref OnAttackEventArgs args) => true;

    }
}
