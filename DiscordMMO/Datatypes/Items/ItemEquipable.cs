using DiscordMMO.Datatypes.Inventories;

namespace DiscordMMO.Datatypes.Items
{
    public abstract class ItemEquipable : Item
    {

        public abstract PlayerEquipmentSlot slot { get; }

    }
}
