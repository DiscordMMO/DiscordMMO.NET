using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordMMO.Datatypes;
using Discord;
using System.Diagnostics;

namespace DiscordMMO.Handlers
{
    public static class DatabaseHandler
    {

        private static readonly string connString;

        public static readonly string BASE_PLAYER_PATH;

        #region Save or overwrite string
        /*
        private const string saveOrOverwriteString = "INSERT INTO users (id, name, currentActionName, currentActionFinishTime, preference_pm, preference_mention, inventory, equipment) " +
    "VALUES(@id, @name, @actionName, @actionTime, @pm, @mention, @inventory, @equipment)" +
    "ON DUPLICATE KEY UPDATE id=@id, name=@name, currentActionName=@actionName, " +
    "currentActionFinishTime=@actionTime, preference_pm=@pm, preference_mention=@mention, inventory=@inventory, equipment=@equipment";
        */
        #endregion

        static DatabaseHandler()
        {

            BASE_PLAYER_PATH = Environment.GetEnvironmentVariable("DISCORDMMO_USERDATA") + @"\Users\";

            #region Deprecated
            /*
            string username, password;
            ConfigHelper.SetConfigPath(@"dangerous.cfg");
            username = ConfigHelper.GetValue("sql_username");
            password = ConfigHelper.GetValue("sql_password");


            // TODO: Move the database to a dedicated database and use that ip instead
            connString = ($"user={username};" +
                $"password={password};server=localhost;" +
                $"Database=discord_mmo_net;" +
                $"port=3306");
                */
#endregion
        }

        public static async Task<bool> CanFetchPlayer(IUser user)
        {
            return await CanFetchPlayer(user.Id);
        }

        public static async Task<bool> CanFetchPlayer(ulong id)
        {

            return File.Exists(BASE_PLAYER_PATH + id);
      
        }


        public static async Task<Player> GetOrFetchPlayer(IUser user, DiscordSocketClient client)
        {
            return await GetOrFetchPlayer(user.Id, client);
        }

        public static async Task<Player> GetOrFetchPlayer(ulong id, DiscordSocketClient client)
        {

            if (PlayerHandler.HasPlayer(client.GetUser(id)))
            {
                return PlayerHandler.GetPlayer(client.GetUser(id));
            }

            string playerPath = BASE_PLAYER_PATH + id + ".xml";

            if (!File.Exists(playerPath))
            {
                return null;
            }

            using (StreamReader file = new StreamReader(playerPath))
            {
                return SerializationHandler.GetSerializer<Player>().Deserialize(file) as Player;
            }

            #region Deprecated
            /*
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

                    string actionName;
                    List<object> actionParams = new List<object>();

                    actionParams.Add(p);

                    if (actionNameFull.Contains("."))
                    {
                        actionName = actionNameFull.Split('.')[0];
                        List<object> param = actionNameFull.Split('.')[1].Replace("(", "").Replace(")", "").Split('.').ToList<object>();

                        param.ForEach(s => s = SerializationHandler.BreakDown(s));

                        actionParams.AddRange(param);
                    }
                    else
                    {
                        actionName = actionNameFull;
                    }

                    p.SetAction(Action.GetActionInstanceFromName(actionName, actionParams.ToArray()), false, true);

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
            */
#endregion
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
            using (MemoryStream mem = new MemoryStream())
            using (FileStream file = new FileStream(BASE_PLAYER_PATH + player.ID + ".xml", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete | FileShare.ReadWrite, bufferSize: 1000000000, useAsync: true))
            {
                SerializationHandler.GetSerializer<Player>().Serialize(mem, player);
                byte[] b = mem.ToArray();
                await file.WriteAsync(b, 0, b.Length);
            }
        }

    }
}
