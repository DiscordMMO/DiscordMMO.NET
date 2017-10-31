using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.Inventories
{
    public class InventoryLootPile : List<(ItemStack stack, DateTime updated)>
    {

        /// <summary>
        /// The amount of time, in seconds, that items stay in the loot pile for until they disappear
        /// </summary>
        public const int lootLingerTime = 300;

        public void Update()
        {
            List<(ItemStack stack, DateTime updated)> toRemove = new List<(ItemStack stack, DateTime updated)>();
            foreach ((ItemStack stack, DateTime updated) item in this)
            {
                if (item.updated.AddSeconds(lootLingerTime) >= DateTime.Now)
                {
                    toRemove.Add(item);
                }
            }

            foreach ((ItemStack stack, DateTime updated) item in toRemove)
            {
                Remove(item);
            }
        }

        public void Add(ItemStack item)
        {
            if (this.Count(x => x.stack.itemType == item.itemType) > 0)
            {
                // Find the first stack that matches the type of the input stack
                int indexOfMatching = FindIndex(x => x.stack.itemType == item.itemType);

                // Add the size of the input stack to that first stack
                ItemStack newStack = this[indexOfMatching].stack;
                newStack.count += item.count;

                // Initialize a new stack
                (ItemStack stack, DateTime updated) newElement = (newStack, DateTime.Now);

                //Insert that new stack
                this[indexOfMatching] = newElement;

                return;
            }

            if (this.Count(x => x.stack.IsEmpty || x.stack == null) > 0)
            {
                this[FindIndex(x => x.stack.IsEmpty || x.stack == null)] = (item, DateTime.Now);
            }
            else
            {
                Add((item, DateTime.Now));
            }


        }

        public List<ItemStack> GetAsItemStacks()
        {
            return this.Select<(ItemStack stack, DateTime), ItemStack>(x => x.stack).ToList();
        }

    }
}
