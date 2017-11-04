using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Entities;
using DiscordMMO.Util;

namespace DiscordMMO.Datatypes.Areas
{
    public class Area
    {

        public virtual string name { get; set; }

        public virtual Direction blockedAt { get; } = Direction.NONE;

        public List<Entity> content { get; protected set; }

        /// <summary>
        ///  Get the time it takes in seconds to move to this area via the direction <paramref name="from"/>
        /// </summary>
        /// <param name="from">The direction to move from</param>
        /// <returns></returns>
        public virtual int GetMoveTime(Direction from)
        {
            return 600;
        }

    }
}
