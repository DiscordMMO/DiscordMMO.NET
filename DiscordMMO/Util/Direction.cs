using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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

    public static class DirectionHelper
    { 
        public static Point GetOffset(this Direction direction)
        {
            int xOffset, yOffset;

            switch (direction)
            {
                case Direction.NORTH:
                    xOffset = 0;
                    yOffset = 1;
                    break;
                case Direction.SOUTH:
                    xOffset = 0;
                    yOffset = -1;
                    break;
                case Direction.EAST:
                    xOffset = 1;
                    yOffset = 0;
                    break;
                case Direction.WEST:
                    xOffset = -1;
                    yOffset = 0;
                    break;
                default:
                    xOffset = 0;
                    yOffset = 0;
                    break;
            }

            return new Point(xOffset, yOffset);
        }

        public static Direction FromString(string directionString)
        {
            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                if (dir.ToString().ToLowerInvariant().Equals(directionString.ToLowerInvariant()))
                    return dir;
            }
            return Direction.NONE;
        }

    }


}
