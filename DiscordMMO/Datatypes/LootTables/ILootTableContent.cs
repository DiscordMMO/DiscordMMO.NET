using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.LootTables
{
    public interface ILootTableContent
    {

        ItemStack GetDrop();

    }
}
