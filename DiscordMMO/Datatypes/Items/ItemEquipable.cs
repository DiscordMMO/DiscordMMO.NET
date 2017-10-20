using DiscordMMO.Datatypes.Inventories;

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

    }
}
