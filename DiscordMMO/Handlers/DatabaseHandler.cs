using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordMMO.Datatypes;
using DiscordMMO.Helpers;
using DiscordMMO.Datatypes.Actions;
using Discord;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using DiscordMMO.Datatypes.Inventories;
using Action = DiscordMMO.Datatypes.Actions.Action;

namespace DiscordMMO.Handlers
{
    public static class DatabaseHandler
    {

        private static readonly string connString;

        #region Save or overwrite string
        private const string saveOrOverwriteString = "INSERT INTO users (id, name, currentActionName, currentActionFinishTime, preference_pm, preference_mention, inventory, equipment) " +
    "VALUES(@id, @name, @actionName, @actionTime, @pm, @mention, @inventory, @equipment)" +
    "ON DUPLICATE KEY UPDATE id=@id, name=@name, currentActionName=@actionName, " +
    "currentActionFinishTime=@actionTime, preference_pm=@pm, preference_mention=@mention, inventory=@inventory, equipment=@equipment";
        #endregion

        static DatabaseHandler()
        {
            string username, password;
            ConfigHelper.SetConfigPath(@"dangerous.cfg");
            username = ConfigHelper.GetValue("sql_username");
            password = ConfigHelper.GetValue("sql_password");


            // TODO: Move the database to a dedicated database and use that ip instead
            connString = ($"user={username};" +
                $"password={password};server=localhost;" +
                $"Database=discord_mmo_net;" +
                $"port=3306");

        }

        public static async Task CheckConnection()
        {

            MySqlConnection connection;

            connection = new MySqlConnection(connString);

            Console.WriteLine("[Database Handler] Testing connection to server");
            Stopwatch watch = Stopwatch.StartNew();
            using (connection)
            {
                await connection.OpenAsync();
            }
            watch.Stop();
            Console.WriteLine($"[Database Handler] First connection took {watch.ElapsedMilliseconds}ms");
        }

        public static async Task<bool> CanFetchPlayer(IUser user)
        {
            return await CanFetchPlayer(user.Id);
        }

        public static async Task<bool> CanFetchPlayer(ulong id)
        {

            MySqlConnection connection;

            connection = new MySqlConnection(connString);

            MySqlCommand getUserFromId;
            MySqlDataReader reader;
            using (connection)
            {
                await connection.OpenAsync();
                getUserFromId = new MySqlCommand("SELECT * FROM users WHERE id = @id", connection);
                getUserFromId.Parameters.AddWithValue("@id", id);
                getUserFromId.Prepare();
                reader = getUserFromId.ExecuteReader();
                using (reader)
                {

                    return reader.HasRows;
                }
            }
        }


        public static async Task<Player> GetOrFetchPlayer(IUser user, DiscordSocketClient client)
        {
            return await GetOrFetchPlayer(user.Id, client);
        }

        public static async Task<Player> GetOrFetchPlayer(ulong id, DiscordSocketClient client)
        {

            // TODO: Add equipment deserialization

            if (PlayerHandler.HasPlayer(client.GetUser(id)))
            {
                return PlayerHandler.GetPlayer(client.GetUser(id));
            }

            MySqlConnection connection;
            
            MySqlCommand getUserFromId;

            connection = new MySqlConnection(connString);
            if (!await CanFetchPlayer(id))
                return null;
            MySqlDataReader reader;
            using (connection)
            {
                getUserFromId = new MySqlCommand("SELECT * FROM users WHERE id = @id", connection);
                await connection.OpenAsync();
                getUserFromId.Parameters.AddWithValue("@id", id);
                getUserFromId.Prepare();
                reader = getUserFromId.ExecuteReader();
                using (reader)
                {
                    if (!reader.Read())
                        return null;
                    IUser user = client.GetUser(id);
                    if (PlayerHandler.HasPlayer(user))
                        return PlayerHandler.GetPlayer(user);
                    Player p = PlayerHandler.CreatePlayer(user, reader.GetString("name"));

                    string actionNameFull = reader.GetString("currentActionName");
                    string actionName = actionNameFull.Split('.')[0];
                    object[] actionParams = actionNameFull.Split('.')[1].Replace("[","").Replace("]","").Split(',');

                    p.SetAction(Action.GetActionInstanceFromName(actionName, p, actionParams), false, true);

                    p.inventory = PlayerInventory.FromString(p, reader.GetString("inventory"));
                    p.equipment = PlayerEquimentInventory.FromString(p, reader.GetString("equipment"));

                    if (p.currentAction is ActionIdle == false)
                    {
                        p.currentAction.SetFinishTime(reader.GetDateTime("currentActionFinishTime"));
                    }

                    p.SetPreference("pm", reader.GetBoolean("preference_pm"));
                    p.SetPreference("mention", reader.GetBoolean("preference_mention"));

                    //await p.Tick();

                    return p;

                }
            }
        }


        public static async Task SaveAllAsync()
        {
            Console.WriteLine("[Database Handler] Starting save");
            var watch = Stopwatch.StartNew();
            List<Task> toSave = new List<Task>();
            foreach (Player p in PlayerHandler.GetPlayers())
            {
                toSave.Add(SaveAsync(p));
            }
            await Task.WhenAll(toSave);
            watch.Stop();
            Console.WriteLine($"[Database Handler] Save done after {watch.ElapsedMilliseconds}ms");
        }

        public static async Task SaveAsync(Player player)
        {
            MySqlConnection connection;

            MySqlCommand saveOrOverwritePlayer;

            connection = new MySqlConnection(connString);

            Console.WriteLine("[Database Handler] Saving player: " + player.playerName);

            using (connection)
            {
                try
                {
                    Console.WriteLine("[Database Handler] Attempting to connect to server");
                    await connection.OpenAsync();
                    saveOrOverwritePlayer = new MySqlCommand(saveOrOverwriteString, connection);
                    saveOrOverwritePlayer.Parameters.AddWithValue("@id", player.user.Id);
                    saveOrOverwritePlayer.Parameters.AddWithValue("@name", player.playerName);
                    saveOrOverwritePlayer.Parameters.AddWithValue("@actionName", player.currentAction.name);
                    saveOrOverwritePlayer.Parameters.AddWithValue("@actionTime", player.currentAction.finishTime);
                    saveOrOverwritePlayer.Parameters.AddWithValue("@inventory", player.inventory.ToString());
                    saveOrOverwritePlayer.Parameters.AddWithValue("@equipment", player.equipment.ToString());
                    saveOrOverwritePlayer.Parameters.AddWithValue("@pm", player.GetPreference<bool>("pm"));
                    saveOrOverwritePlayer.Parameters.AddWithValue("@mention", player.GetPreference<bool>("mention"));
                    saveOrOverwritePlayer.Prepare();
                    await saveOrOverwritePlayer.ExecuteNonQueryAsync();
                }
                catch (Exception e)
                {
                  Console.WriteLine(e.ToString());
                }
            }
        }

    }
}
