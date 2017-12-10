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

        /// <summary>
        /// The amount of ticks per second
        /// </summary>
        private static double tickRate = 2;

        public static Server INSTANCE { get; private set; }

        private static int tickBetweenSaves = 120;
        private static int ticksSinceLastSave = 110;

        /// <summary>
        /// The amount of seconds a player can do nothing for, before their instance gets removed from the player list (ie. they get kicked)
        /// </summary>
        public const int IDLE_TIME = 300;

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
                List<Task> toTick = new List<Task>();
                foreach (Player p in PlayerHandler.GetPlayers())
                {
                    toTick.Add(p.Tick());
                }

                // Tick the message handler
                toTick.Add(MessageHandler.Tick());

                // Make sure everything has been ticked
                await Task.WhenAll(toTick);
                
                // Wait for the next tick to occur
                await Task.Delay((int)(1000 / tickRate));
                ticksSinceLastSave++;
                
                // Save every tickBetweenSaves ticks
                if (ticksSinceLastSave >= tickBetweenSaves && Program.sqlAvailable)
                {
                    // Save everything

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    DatabaseHandler.SaveAllAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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
