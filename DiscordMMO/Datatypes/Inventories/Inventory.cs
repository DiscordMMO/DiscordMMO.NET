using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Items;

namespace DiscordMMO.Datatypes.Inventories
{
    public abstract class Inventory
    {

        public List<ItemStack> items = new List<ItemStack>();


        public virtual bool AddItem(ItemStack item)
        {
            for (int j = 0; j < items.Count; j++)
            {
                ItemStack i = items[j];
                if (i.itemType.itemName.Equals(item.itemType))
                {
                    items[j].count += item.count;
                    return true;
                }
            }
            items.Add(item);
            return true;
        }

        public virtual bool ContainsItem(Item item)
        {
            foreach (ItemStack stack in items)
            {
                if (stack == null || stack.IsEmpty)
                    continue;
                if (stack.itemType.itemName.Equals(item.itemName))
                {
                    return true;
                }
            }
            return false;
        }

        public abstract bool CanAdd(ItemStack item);

    }
}
