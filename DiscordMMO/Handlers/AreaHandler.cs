using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Areas;
using System.Drawing;

namespace DiscordMMO.Handlers
{
    public static class AreaHandler
    {

        static Dictionary<Point, Area> map = new Dictionary<Point, Area>();

        public static void LoadArea(int x, int y, Area tile, bool forced = false, bool silent = false)
        {
            if (map.ContainsKey(new Point(x,y)))
            {
                if (!silent)
                {
                    Console.Write($"Tried to load an area at an alredy loaded position at: ({x},{y})");
                    Console.WriteLine("The area was " + (forced ? "" : "not ") + "loaded");
                }
                if (!forced)
                    return;
            }

            map[new Point(x, y)] = tile;

        }

        public static Area GetArea(int x, int y)
        {
            return map[new Point(x, y)];
        }


    }
}
