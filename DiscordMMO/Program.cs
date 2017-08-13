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

            Server.INSTANCE.Run();

            services = new ServiceCollection().BuildServiceProvider();

            await InstallCommands();

            ItemHandler.Init();

            ConfigHelper.SetConfigPath(@"dangerous.cfg");

            string token = ConfigHelper.GetValue("token");

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(-1);

        }

        public async Task InstallCommands()
        {
            client.MessageReceived += HandleCommand;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message.ToString());
            return Task.CompletedTask;
        }

        public async Task HandleCommand(SocketMessage msgParam)
        {

            var msg = msgParam as SocketUserMessage;
            if (msg == null)
                return;

            int argPos = 0;

            if (!(msg.HasCharPrefix(Modules.COMMAND_PREFIX, ref argPos) || msg.HasMentionPrefix(client.CurrentUser, ref argPos))) return;

            var context = new CommandContext(client, msg);

            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }


        }

    }
}
