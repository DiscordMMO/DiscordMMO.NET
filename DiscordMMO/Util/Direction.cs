using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Util
{
    [Flags]
    public enum Direction
    {
        
        NONE = 0,
        NORTH = 1,
        SOUTH = 2,
        EAST = 4,
        WEST = 8

    }
}
