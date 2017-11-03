using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes
{
    public class ItemStackLootPile
    {

        public ItemStack stack;

        public DateTime lastUpdated;


        public ItemStackLootPile(ItemStack stack)
        {
            this.stack = stack;
        }

        public ItemStackLootPile(ItemStack stack, DateTime lastUpdated)
        {
            this.stack = stack;
            this.lastUpdated = lastUpdated;
        }

        public ItemStackLootPile((ItemStack stack, DateTime lastUpdated) values)
        {
            stack = values.stack;
            lastUpdated = values.lastUpdated;
        }

        public void Refresh()
        {
            lastUpdated = DateTime.Now;
        }

        public static implicit operator ItemStackLootPile((ItemStack stack, DateTime lastUpdated) values)
        {
            return new ItemStackLootPile(values);
        }

    }
}
