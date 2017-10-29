using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes;
using Discord;
using Discord.WebSocket;

namespace DiscordMMO.Handlers
{
    public static class PlayerHandler
    {

        private static Dictionary<IUser, DateTime> lastLogout = new Dictionary<IUser, DateTime>();

        /// <summary>
        /// A list of all currently logged in players
        /// </summary>
        // TODO: Periodically wipe this list to save RAM
        private static List<Player> players = new List<Player>();

        /// <summary>
        /// Check if a user has a logged in player
        /// </summary>
        /// <param name="user">The user to check</param>
        /// <returns><c>True</c> if the user has a logged in player</returns>
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

        /// <summary>
        /// Get a player that is logged in.
        /// NOTE: This only works if the player is logged in. Check <see cref="AttemptLogin(SocketUser)"/> before using.
        /// </summary>
        /// <param name="user">The user that owns the player</param>
        /// <returns>The player of the user, <c>null</c> if not available</returns>
        public static Player GetPlayer(IUser user)
        {
            // Return null if the user has no player
            if (!HasPlayer(user))
                return null;
            foreach (Player p in players)
            {
                if (p.user.Id.Equals(user.Id))
                    return p;
            }
            return null;
        }

        /// <summary>
        /// Create a <see cref="Player"/> instance. Use this instead of the constructor.
        /// </summary>
        /// <param name="user">The user that owns the player</param>
        /// <param name="name">The username of the player to be created</param>
        /// <returns>The <see cref="Player"/> that was created. If the user has a player, that will be returned</returns>
        public static Player CreatePlayer(IUser user, string name)
        {
            if (HasPlayer(user))
            {
                return GetPlayer(user);
            }

            Player player = new Player(user as SocketUser, name);
            players.Add(player);
            return player;
        }

        /// <summary>
        /// Try to login the user
        /// </summary>
        /// <param name="user">The user to log in</param>
        /// <returns><c>True</c> if the player for <paramref name="user"/> is now available</returns>
        public static async Task<(bool success, string errorReason)> AttemptLogin(IUser user)
        {
            if (HasPlayer(user))
                return (true, "");
            if (lastLogout.ContainsKey(user) && lastLogout[user].AddMinutes(1) <= DateTime.Now)
                return (false, "You can only log in 60 seconds after you log out");
            if (!await DatabaseHandler.CanFetchPlayer(user))
                return (false, Modules.NOT_REGISTERED_MSG);
            AddPlayerInstance(await DatabaseHandler.GetOrFetchPlayer(user, Program.client));
            return (true, "");
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

        public static void AddPlayerInstance(Player player)
        {
            if (!players.Contains(player))
            {
                players.Add(player);
            }
        }

        public static List<Player> GetPlayers()
        {
            return players;
        }

        public static void LoggedOut(IUser user)
        {
            lastLogout[user] = DateTime.Now;
        }

    }
}
