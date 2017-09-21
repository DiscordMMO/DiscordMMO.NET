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

        // The amount of ticks per second
        private static double tickRate = 2;

        public static Server INSTANCE { get; private set; }

        private static int tickBetweenSaves = 120;
        private static int ticksSinceLastSave = 110;

        static Server()
        {
            #region Singleton
            if (INSTANCE == null)
                INSTANCE = new Server();
            #endregion
        }

        public async void Run()
        {
            while (!stop)
            {
                // Tick every player
                List<Task> playerTicks = new List<Task>();
                foreach (Player p in PlayerHandler.GetPlayers())
                {
                    playerTicks.Add(p.Tick());
                }
                // Make sure every player has been ticked
                await Task.WhenAll(playerTicks);
                
                // Wait for the next tick to occur
                await Task.Delay((int)(1000 / tickRate));
                ticksSinceLastSave++;
                
                // Save every tickBetweenSaves ticks
                if (ticksSinceLastSave >= tickBetweenSaves && Program.sqlAvailable)
                {
                    // Save everything
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
