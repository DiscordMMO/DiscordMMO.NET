using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.LootTables
{
    public class LootTable : ILootTableContent
    {

        protected static Random random = new Random();

        // Higher weight = more likely
        public virtual HashSet<(ILootTableContent drop, int weight)> drops { get; set; } = new HashSet<(ILootTableContent, int)>();

        public LootTable(params (ILootTableContent drop, int weight)[] drops)
        {
            this.drops = new HashSet<(ILootTableContent drop, int weight)>(drops);
        }

        public virtual ItemStack GetDrop()
        {
            // Stolen from https://stackoverflow.com/questions/56692/random-weighted-choice

            int totalWeight = drops.Sum(d => d.weight);

            float r = random.Next(0, totalWeight);

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
