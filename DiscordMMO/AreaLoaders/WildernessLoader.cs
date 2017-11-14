using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Handlers;
using DiscordMMO.Datatypes.Areas;
using DiscordMMO.Factories;
using DiscordMMO.Datatypes.Entities;

namespace DiscordMMO.AreaLoaders
{
    public class WildernessLoader : IAreaLoader
    {
        public void Load()
        {
            AreaWilderness area = new AreaWilderness();
            area.content.Add(EntityFactory.CreateFightable<EntityGoblin>());
            AreaHandler.LoadArea(0, 1, area);
        }

        public void PostLoad()
        {
        }

        public void PreLoad()
        {
        }
    }
}
