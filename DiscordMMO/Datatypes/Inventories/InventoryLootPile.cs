using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.Inventories
{
    public class InventoryLootPile
    {

        /// <summary>
        /// The amount of time, in seconds, that items stay in the loot pile for until they disappear
        /// </summary>
        public const int lootLingerTime = 300;

        private List<ItemStackLootPile> items = new List<ItemStackLootPile>();

        public List<ItemStackLootPile> Items => items;

        public List<ItemStack> ItemStacks => items.Select(x => x.stack).ToList();

        public void Update()
        {
            List<ItemStackLootPile> toRemove = new List<ItemStackLootPile>();
            foreach (ItemStackLootPile item in items)
            {
                if (item.lastUpdated.AddSeconds(lootLingerTime) <= DateTime.Now)
                {
                    toRemove.Add(item);
                }
            }

            foreach (ItemStackLootPile item in toRemove)
            {
                items.Remove(item);
            }
        }

        public void Add(ItemStack item)
        {
            if (items.Count(x => x.stack.itemType == item.itemType) > 0)
            {
                // Find the first stack that matches the type of the input stack
                int indexOfMatching = items.FindIndex(x => x.stack.itemType == item.itemType);

                // Add the size of the input stack to that first stack
                items[indexOfMatching].stack.count += item.count;

                // Refresh the stack
                items[indexOfMatching].Refresh();

                return;
            }

            if (items.Count(x => x.stack.IsEmpty || x.stack == null) > 0)
            {
                items[items.FindIndex(x => x.stack.IsEmpty || x.stack == null)] = (item, DateTime.Now);
            }
            else
            {
                items.Add((item, DateTime.Now));
            }


        }

        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }

    }
}
