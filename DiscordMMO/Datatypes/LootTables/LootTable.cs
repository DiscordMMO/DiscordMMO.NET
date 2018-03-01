using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Util;

namespace DiscordMMO.Datatypes.LootTables
{
    public class LootTable : ILootTableContent
    {


        // Higher weight = more likely
        public virtual HashSet<(ILootTableContent drop, float weight)> drops { get; set; } = new HashSet<(ILootTableContent, float)>();

        public LootTable(params (ILootTableContent drop, float weight)[] drops)
        {
            this.drops = new HashSet<(ILootTableContent drop, float weight)>(drops);
        }

        public virtual ItemStack GetDrop()
        {
            // Partially stolen from https://stackoverflow.com/questions/56692/random-weighted-choice

            float totalWeight = drops.Sum(d => d.weight);

            float r;

            // Wait for the rng to be available
            lock (NumberUtil.randomLock)
            {
                r = NumberUtil.random.Range(0, totalWeight);
            }

            ILootTableContent drop = null;

            foreach (var d in drops)
            {
                if (r < d.weight)
                {
                    drop = d.drop;
                    break;
                }
                r -= d.weight;
            }

            return drop.GetDrop();

        }

    }
}
