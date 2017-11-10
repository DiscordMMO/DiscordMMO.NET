using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Entities;
using DiscordMMO.Util;
using DiscordMMO.Handlers;
using System.Xml.Serialization;

namespace DiscordMMO.Datatypes.Areas
{
    [XmlRoot]
    [AlsoRequires(typeof(Entity))]
    public class Area
    {

        [XmlElement]
        public virtual string name { get; set; }

        [XmlIgnore]
        public virtual string displayName { get; set; }

        [XmlElement]
        public virtual Direction blockedAt { get; set; } = Direction.NONE;

        [XmlElement]
        public List<Entity> content { get; set; }

        [XmlElement]
        public int x, y;

        [XmlIgnore]
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
