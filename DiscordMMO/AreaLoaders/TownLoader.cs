using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Handlers;
using DiscordMMO.Datatypes.Areas.Towns;

namespace DiscordMMO.AreaLoaders
{
    public class TownLoader : IAreaLoader
    {

        public void PreLoad()
        {
        }

        public void Load()
        {
            AreaHandler.LoadArea(0, 0, new AreaStartertown());
        }

        public void PostLoad()
        {
        }
    }
}
