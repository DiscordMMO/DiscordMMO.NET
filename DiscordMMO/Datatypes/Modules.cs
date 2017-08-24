﻿using Discord;
using Discord.Commands;
using DiscordMMO.Consts;
using DiscordMMO.Datatypes.Actions;
using DiscordMMO.Datatypes.Preferences;
using DiscordMMO.Handlers;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Inventories;
using DiscordMMO.Helpers;
using Action = DiscordMMO.Datatypes.Actions.Action;

namespace DiscordMMO.Datatypes
{
    public static class Modules
    {
        public static readonly string COMMAND_PREFIX;
        public const string ALREADY_REGISTERED_MSG = "You are already registered";
        public const string NOT_REGISTERED_MSG = "You are not registered, register with $register [name]";
        public const string NOT_REGISTERED_THIRD_PERSON_NO_NAME = " is not registered";
        public const string LOGGED_IN_AS = "You have been logged in as ";

        public const string REGISTERED_FORMAT = "You have registered with the name {0}\n" +
                                                "See available commands with $help";

        public const string ALREADY_ACTIVE_TIME_LEFT_FORMAT = "You are already doing something. You will be done in {0}";

        public const string PREF_MSG_START = "To set a preference use $pref <name> <value>\n" +
                                             "To view the value of a single preference, use $pref <name>\n" +
                                             "Preferences:\n" +
                                             "[Preference name]: [value]\n";

        static Modules()
        {
            ConfigHelper.SetConfigPath("botconfig.cfg");
            COMMAND_PREFIX = ConfigHelper.GetValue(ConfigHelper.GetValue("command_prefix"));
        }

        public static string NOT_REGISTERED_THIRD_PERSON(string name)
        {
            return name + NOT_REGISTERED_THIRD_PERSON_NO_NAME;
        }

    }
    public class NoPrefix : ModuleBase
    {


        [Command("ping"), Summary("Pong")]
        public async Task Ping()
        {
            await ReplyAsync("Pong!");
        }

        [Command("register"), Summary("Register with the name, if one is given")]
        public async Task Register([Summary("The name to register with")] string name = null)
        {
            if (name == null || name.Trim().Equals(""))
            {
                name = Context.User.Username;
            }
            if (await PlayerHandler.AttemptLogin(Context.User))
            {
                await ReplyAsync($"{Context.User.Mention}: {Modules.ALREADY_REGISTERED_MSG}");
                return;
            }
            else
            {
                PlayerHandler.CreatePlayer(Context.User, name);
                await ReplyAsync($"{Context.User.Mention}: {String.Format(Modules.REGISTERED_FORMAT, name)}");
            }

        }

        [Command("status"), Summary("Gets what the optional user is doing")]
        public async Task Status(IUser user = null)
        {
            if (user == null)
            {
                if (!await PlayerHandler.AttemptLogin(Context.User))
                {
                    await ReplyAsync(Context.User.Mention + ": " + Modules.NOT_REGISTERED_MSG);
                    return;
                }
                user = Context.User;
                Player player = PlayerHandler.GetPlayer(user);
                string doneIn = String.Format(Action.DONE_IN_FORMAT, ((DateTime)player.currentAction.finishTime - DateTime.Now));
                await ReplyAsync(Context.User.Mention + ": " + player.currentAction.GetActiveFormattingSecondPerson() +
                    ((player.currentAction is ActionIdle) ? "" : doneIn));
                return;
            }
            if (!await PlayerHandler.AttemptLogin(user))
            {
                await ReplyAsync(Context.User.Mention + ": " + Modules.NOT_REGISTERED_THIRD_PERSON(user.Username));
                return;
            }
            Player target = PlayerHandler.GetPlayer(user);
            await ReplyAsync(Context.User.Mention + ": " + target.currentAction.GetActiveFormattingThridPerson(false));

        }

        [Command("chop"), Summary("Starts chopping wood")]
        public async Task Chop()
        {
            if (!await PlayerHandler.AttemptLogin(Context.User))
            {
                await ReplyAsync(Context.User.Mention + ": " + Modules.NOT_REGISTERED_MSG);
                return;
            }

            Player p = PlayerHandler.GetPlayer(Context.User);

            if (!p.IsIdle)
            {
                await ReplyAsync(Context.User.Mention + ": " + String.Format(Modules.ALREADY_ACTIVE_TIME_LEFT_FORMAT, p.currentAction.finishTime - DateTime.Now));
                return;
            }

            p.SetAction(new ActionChopWood(p), p.GetPreference<bool>("pm"), false);
            Action a = p.currentAction;
            await ReplyAsync(Context.User.Mention + ": " + a.GetStartedFormattingSecondPerson());
        }

        [Command("pref"), Summary("View or edit preferences")]
        public async Task Prefs(string prefName = null, string value = null)
        {
            if (!await PlayerHandler.AttemptLogin(Context.User))
            {
                await ReplyAsync(Context.User.Mention + ": " + Modules.NOT_REGISTERED_MSG);
                return;
            }
            Player p = PlayerHandler.GetPlayer(Context.User);
            if (String.IsNullOrWhiteSpace(value) && String.IsNullOrWhiteSpace(prefName))
            {
                StringBuilder outp = new StringBuilder(Modules.PREF_MSG_START);
                foreach (string key in p.GetPreferences().Keys)
                {
                    IPreference pref = p.GetPreferences()[key];
                    outp.Append(key + ": " + pref + "\n");
                }
                await ReplyAsync(outp.ToString());
                return;
            }
            else if (!String.IsNullOrWhiteSpace(prefName) && String.IsNullOrWhiteSpace(value))
            {
                IPreference pref = p.GetPreferences()[prefName];

                if (pref == null)
                {
                    await ReplyAsync(Context.User.Mention + ": That preference does not exist");
                    return;
                }

                await ReplyAsync(Context.User.Mention + ": " + prefName + " is " + pref);
                return;
            }
            else if (!String.IsNullOrWhiteSpace(prefName) && !String.IsNullOrWhiteSpace(value))
            {
                IPreference pref = p.GetPreferences()[prefName];
                Type t = pref.type;
                var toSet = Convert.ChangeType(value, t);
                p.SetPreferenceWithType(prefName, toSet, t);
            }

        }

        [Command("login")]
        public async Task LoginCommand()
        {
            if (!await PlayerHandler.AttemptLogin(Context.User))
            {
                await ReplyAsync(Context.User.Mention + ": " + Modules.NOT_REGISTERED_MSG);
                return;
            }
            Player p = PlayerHandler.GetPlayer(Context.User);
            await ReplyAsync(Context.User.Mention + ": " + Modules.LOGGED_IN_AS + p.name);
        }

        [Command("inventory")]
        public async Task InventoryCommand()
        {


            if (!await PlayerHandler.AttemptLogin(Context.User))
            {
                await ReplyAsync(Context.User.Mention + ": " + Modules.NOT_REGISTERED_MSG);
                return;
            }

            Player player = PlayerHandler.GetPlayer(Context.User);
            StringBuilder outp = new StringBuilder($"Inventory for {Context.User.Mention}\n");
            outp.Append($"{player.inventory.FreeSpaces}/{player.inventory.size} Slots available\n");
            int i = 0;
            foreach (ItemStack stack in player.inventory.items)
            {
                if (i % 5 == 0)
                {
                    outp.Append("\n");
                }
                if (stack == null)
                {
                    outp.Append(ItemStack.empty.ToString());
                }
                else
                {
                    outp.Append(stack.ToString() + " ");
                }

                i++;
            }
            await ReplyAsync(outp.ToString());
        }

    }

    [Group("db")]
    [RequireOwner]
    public class DebugModules : ModuleBase
    {

        protected override void BeforeExecute(CommandInfo command)
        {
            if (!UserConsts.adminIDs.Contains(Context.User.Id))
                return;
            base.BeforeExecute(command);

        }

        [Command("save")]
        public async Task SaveCommand()
        {
            await DatabaseHandler.SaveAllAsync();
        }

        [Command("players")]
        public async Task ListPlayers()
        {
            StringBuilder outp = new StringBuilder("Players: \n");

            foreach (Player p in PlayerHandler.GetPlayers())
            {
                outp.Append(p.name + ": " + p.currentAction + "\n");
            }

            await ReplyAsync(outp.ToString());
        }

        [Command("reply")]
        public async Task Reply()
        {
            if (! await PlayerHandler.AttemptLogin(Context.User))
                return;
            Player p = PlayerHandler.GetPlayer(Context.User);
            string eq = p.equipment.ToString();
            await ReplyAsync(p.equipment.ToString());
            PlayerEquimentInventory.FromString(p, eq);
        }

    }


}
