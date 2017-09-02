using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes;
using Discord;

namespace DiscordMMO.Handlers
{
    public static class PlayerHandler
    {

        private static List<Player> players = new List<Player>();

        public static bool HasPlayer(IUser user)
        {
            foreach (Player p in players)
            {
                if (p.user.Id.Equals(user.Id))
                {
                    return true;
                }
            }
            return false;
        }

        public static Player GetPlayer(IUser user)
        {
            if (!HasPlayer(user))
                return null;
            foreach (Player p in players)
            {
                if (p.user.Id.Equals(user.Id))
                    return p;
            }
            return null;
        }

        public static Player CreatePlayer(IUser user, string name)
        {
            if (HasPlayer(user))
            {
                return null;
            }

            Player player = new Player(user, name);
            players.Add(player);
            return player;
        }

        public static async Task<bool> AttemptLogin(IUser user)
        {
            if (HasPlayer(user))
                return true;
            if (!Program.sqlAvailable)
                return false;
            if (!await DatabaseHandler.CanFetchPlayer(user))
                return false;
            await DatabaseHandler.GetOrFetchPlayer(user, Program.client);
            return true;
        }

        public static Player CreatePlayer(IUser user)
        {
            Console.WriteLine(user.Username);
            return CreatePlayer(user, user.Username);
        }

        public static void RemovePlayerInstance(IUser user)
        {
            RemovePlayerInstance(GetPlayer(user));
        }

        public static void RemovePlayerInstance(Player player)
        {
            players.Remove(player);
        }

        public static List<Player> GetPlayers()
        {
            return players;
        }

    }
}
