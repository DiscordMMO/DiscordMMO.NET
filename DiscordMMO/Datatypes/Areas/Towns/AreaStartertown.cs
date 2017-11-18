using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Entities;

namespace DiscordMMO.Datatypes.Areas.Towns
{
    public class AreaStartertown : Area
    {

        public override string name => "town_startertown";
        public override string displayName => "Startertown";

        public AreaStartertown()
        {
            content.Add(new EntityMan());
        }

    }
}
