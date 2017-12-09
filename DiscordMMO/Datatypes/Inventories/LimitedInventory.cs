using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Items;

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
            // Check if there are any empty slots in the inventory
            if (FreeSpaces <= 0)
            {
                // If there are no empty slots in the inventory, and the item is not stackable, don't add the item
                if (!item.itemType.stackable)
                    return false;
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
                // Check if the item is stackable
                if (!item.itemType.stackable)
                {
                    items[GetFirstEmptyIndex()] = item;
                    return true;
                }
                if (ContainsItem(item.itemType))
                {
                    items.First(x => x.itemType.itemName.Equals(item.itemType.itemName)).count += item.count;
                    return true;
                }
                else
                {
                    items[GetFirstEmptyIndex()] = item;
                    return true;
                }
            }
        }

        public override bool CanAdd(ItemStack item) => items.Count >= size || (ContainsItem(item.itemType) && item.itemType.stackable);

        /// <summary>
        /// Get the index of the first empty slot in the inventory
        /// </summary>
        /// <returns>The index of the first element in the items list, that is an empty item. If there are no empty slots, <b>-1</b> will be returned</returns>
        public int GetFirstEmptyIndex()
        {
            if (!items.Exists(item => item.itemType is ItemEmpty || item.IsEmpty))
                return -1;
            return items.FindIndex(item => item.itemType is ItemEmpty || item.IsEmpty);
        }

        public virtual void Clear()
        {
            ReplaceAllWith(ItemStack.empty);
        }

        public virtual void ReplaceAllWith(ItemStack item)
        {
            for (int i = 0; i < size; i++)
            {
                items[i] = item;
            }
        }

    }
}
