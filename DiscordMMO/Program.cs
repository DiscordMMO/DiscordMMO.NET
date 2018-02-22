using Discord;
using Discord.WebSocket;
using DiscordMMO.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Discord.Commands;
using System.Reflection;
using DiscordMMO.Handlers;

namespace DiscordMMO
{
    public class Program
    {

        public static readonly CultureInfo culture = CultureInfo.InvariantCulture;

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
            // Set the culture to the given culture
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            // Initialize the logger
            Logger.Init();

            // Init the client
            client = new DiscordSocketClient();

            // Init the command service
            commands = new CommandService();

            // Add the logging handler
            client.Log += Log;

            // Read the config
            ConfigHelper.SetConfigPath("botconfig.cfg");


            // Initialize handlers
            Task initAll = InitAll();

            // Initialize services
            services = new ServiceCollection().BuildServiceProvider();

            // Install commands
            await InstallCommands();

            // Read the bot token
            ConfigHelper.SetConfigPath(@"dangerous.cfg");

            string token = ConfigHelper.GetValue("token");

            await initAll;

            // Start the bot itself
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(-1);

        }

        public async Task InitAll()
        {

            Task[] init = new Task[]
            {
            /*
            ItemHandler.Init(),
            Datatypes.Actions.Action.Init(),
            EntityHandler.Init(),
            SerializationHandler.Init(),
            AreaLoaderHandler.Init(),
            AreaHandler.Init()
            */
            HandlerHandler.Init()
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
                if (typeof(ExecuteResult).IsAssignableFrom(result.GetType()))
                {
                    Logger.Log("An error occurred", LogSeverity.Error);
                    ExecuteResult res = (ExecuteResult)result;
                    Logger.Log(res.Exception.ToString(), LogSeverity.Error);
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
            }


        }

    }
}
