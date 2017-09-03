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

        public ItemStack this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value; 
            }
        }

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

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            foreach (ItemStack i in items)
            {
                b.Append($"{i.ToString()};");
            }
            string o = b.ToString();
            return o.Remove(o.Length-1);
        }

        public abstract bool CanAdd(ItemStack item);

    }
}
