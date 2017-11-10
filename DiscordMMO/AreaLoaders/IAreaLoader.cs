using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.AreaLoaders
{
    public interface IAreaLoader
    {

        void PreLoad();
        void Load();
        void PostLoad();

    }
}
