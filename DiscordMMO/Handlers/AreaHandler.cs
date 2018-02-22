using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Areas;
using System.Drawing;
using Discord;

namespace DiscordMMO.Handlers
{
    [Handler(25)]
    public static class AreaHandler
    {
        public const int fillSize = 128;

        static Dictionary<Point, Area> map = new Dictionary<Point, Area>();

        public delegate void BeforeLoadEvent();
        public delegate void LoadEvent();
        public delegate void AfterLoadEvent();

        public static event BeforeLoadEvent beforeLoadEvent;
        public static event LoadEvent loadEvent;
        public static event AfterLoadEvent afterLoadEvent;

        public static void LoadArea(int x, int y, Area tile, bool forced = false, bool silent = false)
        {
            // Make sure that the tile is in the right position
            tile.position = new Point(x, y);

            if (map.ContainsKey(new Point(x,y)))
            {
                if (!silent)
                {
                    Logger.Log($"Tried to load an area at an alredy loaded position at: ({x},{y})", LogSeverity.Warning);
                    Logger.Log("The area was " + (forced ? "" : "not ") + "loaded", LogSeverity.Warning);
                }
                if (!forced)
                    return;
            }

            map[new Point(x, y)] = tile;

        }

        [InitMethod]
        public static async Task Init()
        {
            LoadAll();
        }

        private static void LoadAll()
        {
            BeforeLoad();
            Load();
            AfterLoad();
        }

        private static void BeforeLoad()
        {
            beforeLoadEvent?.Invoke();
        }

        private static void Load()
        {
            loadEvent?.Invoke();
        }

        private static void AfterLoad()
        {
            afterLoadEvent?.Invoke();
            for (int x = -fillSize; x < fillSize; x++)
            {
                for (int y = -fillSize; y < fillSize; y++)
                {
                    LoadArea(x, y, new AreaWilderness { isUnloaded = true }, forced:false, silent:true);
                }
            }
        }

        public static Area GetArea(int x, int y)
        {
            return GetArea(new Point(x, y));
        }

        public static Area GetArea(Point position)
        {
            return map[position];
        }

    }
}
