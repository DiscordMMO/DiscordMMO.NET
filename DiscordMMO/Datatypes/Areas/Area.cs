using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Entities;
using DiscordMMO.Util;

namespace DiscordMMO.Datatypes.Areas
{
    public class Area
    {

        public virtual string name { get; protected set; }

        public virtual string displayName { get; protected set; }

        public virtual Direction blockedAt { get; set; } = Direction.NONE;

        public List<Entity> content { get; protected set; }

        public int x, y;

        public Point position
        {
            get
            {
                return new Point(x, y);
            }
            set
            {
                x = value.X;
                y = value.Y;
            }
        }


        /// <summary>
        ///  Get the time it takes in seconds to move to this area via the direction <paramref name="from"/>
        /// </summary>
        /// <param name="from">The direction to move from</param>
        /// <returns></returns>
        public virtual int GetMoveTime(Direction from)
        {
            return 600;
        }

        public virtual void OnTick(Player player)
        {
             
        }

    }
}
