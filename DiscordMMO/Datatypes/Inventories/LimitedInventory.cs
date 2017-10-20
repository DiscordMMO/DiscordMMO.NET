using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.Inventories
{
    public abstract class LimitedInventory : Inventory
    {

        public abstract int size { get; }

        public virtual int FreeSpaces
        {
            get
            {
                int count = 0;
                foreach (ItemStack stack in items)
                {
                    if (stack == null || stack.IsEmpty)
                        count++;
                }
                return count;
            }
        }

        public LimitedInventory() { }

        public LimitedInventory(bool fill)
        {
            if (fill)
            {
                for (int i = 0; i < size; i++)
                {
                    items.Add(ItemStack.empty);
                }
            }
        }

        public override bool AddItem(ItemStack item)
        {
            if (FreeSpaces <= 0)
            {
                if (ContainsItem(item.itemType))
                {
                    items.First(x => x.itemType.itemName.Equals(item.itemType.itemName)).count += item.count;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (ContainsItem(item.itemType))
                {
                    items.First(x => x.itemType.itemName.Equals(item.itemType.itemName)).count += item.count;
                    return true;
                }
                else
                {
                    for (int i = 0; i < size; i++)
                    {
                        if (items[i].IsEmpty)
                        {
                            items[i] = item;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override bool CanAdd(ItemStack item) => items.Count >= size || ContainsItem(item.itemType);


    }
}
