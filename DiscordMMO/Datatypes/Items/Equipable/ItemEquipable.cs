using DiscordMMO.Datatypes.Inventories;
using DiscordMMO.Datatypes.Items.ItemEffects;
using DiscordMMO.Util;
using System.Collections.Generic;

namespace DiscordMMO.Datatypes.Items.Equipable
{
    public abstract class ItemEquipable : Item
    {

        public abstract PlayerEquipmentSlot slot { get; }

        public List<ItemEffect> itemEffects = new List<ItemEffect>();

        public virtual void OnEquip(Player player)
        {
            foreach(ItemEffect effect in itemEffects)
            {
                effect.OnEquipped(player);
            }
        }

        public virtual void WhileEquipped(Player player)
        {
            foreach (ItemEffect effect in itemEffects)
            {
                effect.WhileEquipped(player);
            }
        }

        public virtual void OnUnequip(Player player)
        {
            foreach (ItemEffect effect in itemEffects)
            {
                effect.OnUnequip(player);
            }
        }

        public virtual bool CanAttack(ref OnAttackEventArgs args) => true;

    }
}
