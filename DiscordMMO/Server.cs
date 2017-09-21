using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using DiscordMMO.Handlers;
using DiscordMMO.Datatypes;
using DiscordMMO.Datatypes.Actions;

namespace DiscordMMO
{
    public sealed class Server
    {

        private volatile bool stop = false;

        private static double tickRate = 2;

        public static Server INSTANCE { get; private set; }

        private static int tickBetweenSaves = 120;
        private static int ticksSinceLastSave = 110;

        static Server()
        {
            if (INSTANCE == null)
                INSTANCE = new Server();
        }

        public async void Run()
        {
            while (!stop)
            {
                List<Task> playerTicks = new List<Task>();
                foreach (Player p in PlayerHandler.GetPlayers())
                {
                    playerTicks.Add(p.Tick());
                }
                await Task.WhenAll(playerTicks);
                await Task.Delay((int)(1000 / tickRate));
                ticksSinceLastSave++;
                if (ticksSinceLastSave >= tickBetweenSaves && Program.sqlAvailable)
                {
                    DatabaseHandler.SaveAllAsync();
                    ticksSinceLastSave = 0;
                }

            }
        }

        public void Stop()
        {
            stop = true;
        }

    }
}
