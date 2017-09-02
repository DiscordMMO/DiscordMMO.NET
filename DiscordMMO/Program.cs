using Discord;
using Discord.WebSocket;
using DiscordMMO.Datatypes;
using DiscordMMO.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Discord.Commands;
using System.Reflection;
using DiscordMMO.Handlers;

namespace DiscordMMO
{
    public class Program
    {

        public static bool sqlAvailable { get; private set; }

        public static DiscordSocketClient client { get; private set; }
        private CommandService commands;

        public IServiceProvider services { get; private set; }

        static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        static Program()
        {

        }

        public async Task Start()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();

            client.Log += Log;

            ConfigHelper.SetConfigPath("botconfig.cfg");

            sqlAvailable = bool.Parse(ConfigHelper.GetValue("sql_available"));

            if (sqlAvailable)
            {

                var databaseConnectionTimeout = TimeSpan.FromSeconds(30);

                var databaseConnectionTest = DatabaseHandler.CheckConnection();

                if (await Task.WhenAny(databaseConnectionTest, Task.Delay(databaseConnectionTimeout)) == databaseConnectionTest)
                {
                    Console.WriteLine("Database connection OK");
                }
                else
                {
                    Console.WriteLine($"Database connection failed after {databaseConnectionTimeout.Seconds} seconds");
                    Console.ReadKey();
                    Environment.Exit(1);
                }
            }

            InitAll();

            services = new ServiceCollection().BuildServiceProvider();

            await InstallCommands();

            ConfigHelper.SetConfigPath(@"dangerous.cfg");

            string token = ConfigHelper.GetValue("token");

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(-1);

        }

        public async Task InitAll()
        {

            Task[] init = new Task[] 
            {
            ItemHandler.Init(),
            Datatypes.Actions.Action.Init(),
            EntityHandler.Init()
            };

            await Task.WhenAll(init);

            Server.INSTANCE.Run();
        }

        public async Task InstallCommands()
        {
            client.MessageReceived += HandleCommand;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private Task Log(LogMessage msg)
        {
            if (msg.Message != null)
            {
                Console.WriteLine(msg.Message.ToString());
            }
            else
            {
                Console.WriteLine(msg.ToString());
            }
            return Task.CompletedTask;
        }

        public async Task HandleCommand(SocketMessage msgParam)
        {

            var msg = msgParam as SocketUserMessage;
            if (msg == null)
                return;

            int argPos = 0;

            if (!(msg.HasStringPrefix(Modules.COMMAND_PREFIX, ref argPos) || msg.HasMentionPrefix(client.CurrentUser, ref argPos))) return;

            var context = new CommandContext(client, msg);

            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
            {
                if (result.ErrorReason != "Unknown command.")
                {
                    Console.WriteLine("An error occurred");
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
            }


        }

    }
}
