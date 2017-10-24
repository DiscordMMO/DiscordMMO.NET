using Discord;
using Discord.Commands;
using DiscordMMO.Consts;
using DiscordMMO.Datatypes.Actions;
using DiscordMMO.Datatypes.Preferences;
using DiscordMMO.Handlers;
using System.Security.AccessControl;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Inventories;
using DiscordMMO.Helpers;
using DiscordMMO.Datatypes;
using DiscordMMO.Datatypes.Items;
using DiscordMMO.Datatypes.Entities;
using Action = DiscordMMO.Datatypes.Actions.Action;
using Discord.WebSocket;

namespace DiscordMMO
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
            COMMAND_PREFIX = ConfigHelper.GetValue("command_prefix");

        }

        public static string NOT_REGISTERED_THIRD_PERSON(string name)
        {
            return name + NOT_REGISTERED_THIRD_PERSON_NO_NAME;
        }

    }
    public class NoPrefix : ModuleBase
    {

        [Command("register"), Summary("Register with the name, if one is given")]
        public async Task Register([Summary("The name to register with")] string name = null)
        {
            // Check if no name parameter was given
            if (name == null || name.Trim().Equals(""))
            {
                // Set the players username to the discord username of the sender
                name = Context.User.Username;
            }
            // Check if the player already has a user
            if (await PlayerHandler.AttemptLogin(Context.User as SocketUser))
            {
                // Notify the player that they already have player
                await ReplyAsync($"{Context.User.Username}: {Modules.ALREADY_REGISTERED_MSG}");
                return;
            }
            else
            {
                // Create the player instance
                Player player = PlayerHandler.CreatePlayer(Context.User as SocketUser, name);

                // Save the player
                DatabaseHandler.SaveAsync(player);

                // Notify the player that they have been registered
                await ReplyAsync($"{Context.User.Username}: {String.Format(Modules.REGISTERED_FORMAT, name)}");
            }

        }

        [Command("status"), Summary("Gets what the optional user is doing")]
        public async Task Status(IUser user = null)
        {

            bool firstPerson = user == null;

            // Check if a user was given
            if (firstPerson)
            {
                // If no user was given, use the sender
                user = Context.User;
            }

            // Check if the player has a user
            if (!await PlayerHandler.AttemptLogin(user))
            {
                // If the player is not registered, notify them
                await ReplyAsync(Context.User.Username + ": " + (firstPerson ? Modules.NOT_REGISTERED_MSG
                    : Modules.NOT_REGISTERED_THIRD_PERSON(user.Username)));
                return;
            }
            Player player = PlayerHandler.GetPlayer(user);

            // Format the done in
            string doneIn = (firstPerson ? " You" : " They") + String.Format(Action.DONE_IN_FORMAT, (player.currentAction.finishTime - DateTime.Now));

            // Message the player with the status
            await ReplyAsync(Context.User.Username + ": " +
                (firstPerson ? player.currentAction.GetActiveFormattingSecondPerson() : player.currentAction.GetActiveFormattingThridPerson(false)) +
                ((!player.currentAction.hasSetFinishTime) ? "" : doneIn));
            return;

        }

        [Command("chop"), Summary("Starts chopping wood")]
        public async Task Chop()
        {
            // Check if the player can log in
            if (!await PlayerHandler.AttemptLogin(Context.User))
            {
                // If they cannot login, notify them
                await ReplyAsync(Context.User.Username + ": " + Modules.NOT_REGISTERED_MSG);
                return;
            }

            Player p = PlayerHandler.GetPlayer(Context.User);

            // Check if the player is already doing something
            if (!p.IsIdle)
            {
                // Notify the player if they are already doing something
                await ReplyAsync(Context.User.Username + ": " + String.Format(Modules.ALREADY_ACTIVE_TIME_LEFT_FORMAT, p.currentAction.finishTime - DateTime.Now));
                return;
            }

            // Set the players action
            p.SetAction(new ActionChopWood(p), p.GetPreference<bool>("pm"), false);

            Action a = p.currentAction;
            // Notify the player
            await ReplyAsync(Context.User.Username + ": " + a.GetStartedFormattingSecondPerson());
        }

        [Command("pref"), Summary("View or edit preferences")]
        public async Task Prefs(string prefName = null, string value = null)
        {

            // Check if the player can log in
            if (!await PlayerHandler.AttemptLogin(Context.User))
            {
                // If they cannot login, notify them
                await ReplyAsync(Context.User.Username + ": " + Modules.NOT_REGISTERED_MSG);
                return;
            }

            Player p = PlayerHandler.GetPlayer(Context.User);

            // Check if neither a preference name or a value was given
            if (String.IsNullOrWhiteSpace(value) && String.IsNullOrWhiteSpace(prefName))
            {
                // Loop through all the preferences and show their value
                StringBuilder outp = new StringBuilder(Modules.PREF_MSG_START);
                foreach (string key in p.GetPreferences().Keys)
                {
                    Preference pref = p.GetPreferences()[key];
                    outp.Append(key + ": " + pref + "\n");
                }
                await ReplyAsync(outp.ToString());
                return;
            }
            // Check if only a preference name was given
            else if (!String.IsNullOrWhiteSpace(prefName) && String.IsNullOrWhiteSpace(value))
            {
                Preference pref = p.GetPreferences()[prefName];

                // Check if the preference does not exist
                if (pref == null)
                {
                    await ReplyAsync(Context.User.Username + ": That preference does not exist");
                    return;
                }

                // Tell the player the value of the preference
                await ReplyAsync(Context.User.Username + ": " + prefName + " is " + pref);
                return;
            }
            // Check if both a preference name and value was given
            else if (!String.IsNullOrWhiteSpace(prefName) && !String.IsNullOrWhiteSpace(value))
            {
                Preference pref = p.GetPreferences()[prefName];

                // Check if the preference does not exist
                if (pref == null)
                {
                    await ReplyAsync(Context.User.Username + ": That preference does not exist");
                    return;
                }


                // Get the type of the preference
                Type t = pref.value.GetType();

                // Create the value object
                var toSet = Convert.ChangeType(value, t);

                // Set the value of the preference
                p.SetPreferenceWithType(prefName, toSet, t);

                // Notify the player that the preference was changed
                await ReplyAsync($"{Context.User.Username}: Set preference {prefName} to {value}");

            }

        }

        [Command("login")]
        public async Task LoginCommand()
        {
            // Check if the player can log in
            if (!await PlayerHandler.AttemptLogin(Context.User))
            {
                // If they cannot login, notify them
                await ReplyAsync(Context.User.Username + ": " + Modules.NOT_REGISTERED_MSG);
                return;
            }

            Player p = PlayerHandler.GetPlayer(Context.User);

            // Notify the player that they have been logged in
            await ReplyAsync(Context.User.Username + ": " + Modules.LOGGED_IN_AS + p.playerName);
        }

        [Command("inventory")]
        public async Task InventoryCommand()
        {

            // Check if the player can log in
            if (!await PlayerHandler.AttemptLogin(Context.User))
            {
                // If they cannot login, notify them
                await ReplyAsync(Context.User.Username + ": " + Modules.NOT_REGISTERED_MSG);
                return;
            }

            Player player = PlayerHandler.GetPlayer(Context.User);

            // Create the string builder for the message
            StringBuilder outp = new StringBuilder($"Inventory for {player.playerName}\n");

            // Append the free spaces to the message
            outp.Append($"{player.inventory.FreeSpaces}/{player.inventory.size} Slots available\n");

            // Loop through all the items
            int i = 0;
            foreach (ItemStack stack in player.inventory.items)
            {
                // Change to a new line every 5 items
                if (i % 5 == 0)
                {
                    outp.Append("\n");
                }
                // If the stack is null, treat it as empty
                if (stack == null)
                {
                    outp.Append(ItemStack.empty.itemType.displayName);
                }
                else
                {
                    outp.Append(stack.ToStringDisplay());
                }

                // Seperate the items by commas, except when the next item will be on a new line
                if (i % 4 != 0 || i == 0)
                {
                    outp.Append(",");
                }

                outp.Append(" ");

                i++;
            }
            await ReplyAsync(outp.ToString());
        }

        [Command("equip"), Summary("Equip an item")]
        public async Task EquipItemCommand(int itemToEqiupIndex)
        {
            // Check if the player can log in
            if (!await PlayerHandler.AttemptLogin(Context.User))
            {
                // If they cannot login, notify them
                await ReplyAsync(Context.User.Username + ": " + Modules.NOT_REGISTERED_MSG);
                return;
            }

            Player p = PlayerHandler.GetPlayer(Context.User);

            if (!Enumerable.Range(1, p.inventory.size).Contains(itemToEqiupIndex))
            {
                await ReplyAsync($"{p.playerName}: That slot does not exist");
                return;
            }

            // Get the item to equip
            // The -1 is to have slots 1-28, rather than 0-27
            ItemStack itemToEquip = p.inventory[itemToEqiupIndex - 1];

            // Check if there is an item in the given slot
            if (itemToEquip == null || itemToEquip.IsEmpty)
            {
                await ReplyAsync(p.playerName + ": That slot is empty");
                return;
            }
            // Check if the item is equipable
            if (itemToEquip.itemType is ItemEquipable == false)
            {
                await ReplyAsync(p.playerName + ": You cannot equip that item");
                return;
            }
            // Equip the item
            p.Equip(itemToEquip);

            // Notify the user that the item was equipped
            await ReplyAsync(p.playerName + ": Equipped " + itemToEquip.itemType.displayName);

        }

        [Command("equipment")]
        public async Task EquipmentCommand()
        {
            // Check if the player can log in
            if (!await PlayerHandler.AttemptLogin(Context.User))
            {
                // If they cannot login, notify them
                await ReplyAsync(Context.User.Username + ": " + Modules.NOT_REGISTERED_MSG);
                return;
            }

            Player player = PlayerHandler.GetPlayer(Context.User);

            // Create a stringbuilder for the output
            StringBuilder outp = new StringBuilder($"Equipment for {player.playerName}\n");

            // Loop through the players equipment
            int i = 0;
            foreach (ItemStack stack in player.equipment.items)
            {
                // Add the name of the slot to the string
                outp.Append(((PlayerEquipmentSlot)i).GetDisplayName() + ": ");

                // If the slot is null, treat the item as empty
                if (stack == null)
                {
                    outp.Append(ItemStack.empty.itemType.displayName);
                }
                else
                {
                    outp.Append(stack.ToStringDisplay() + " ");
                }

                // Change to a new line
                outp.Append("\n");

                i++;
            }
            // Message the player
            await ReplyAsync(outp.ToString());
            
        }

        [Command("fight")]
        public async Task FightCommand()
        {
            // Check if the player can log in
            if (!await PlayerHandler.AttemptLogin(Context.User))
            {
                // If they cannot login, notify them
                await ReplyAsync(Context.User.Username + ": " + Modules.NOT_REGISTERED_MSG);
                return;
            }

            Player player = PlayerHandler.GetPlayer(Context.User);

            // Start a fight against a new EntityGoblin
            player.StartFight(new EntityGoblin(), true);

            // Notify the player
            await ReplyAsync("Started fighting");

        }

        [Command("combat")]
        public async Task CombatCommand()
        {
            // Check if the player can log in
            if (!await PlayerHandler.AttemptLogin(Context.User))
            {
                // If they cannot login, notify them
                await ReplyAsync(Context.User.Username + ": " + Modules.NOT_REGISTERED_MSG);
                return;
            }

            Player player = PlayerHandler.GetPlayer(Context.User);

            if (player.currentAction is ActionFighting == false)
            {
                await ReplyAsync(player.playerName + ": You are not fighting anything");
                return;
            }

            ActionFighting action = player.currentAction as ActionFighting;

            string outp = "Combat information:\n" +
                          $"{player.playerName}: [{player.health}/{player.maxHealth}]\n" +
                          $"{action.fighting.name}: [{action.fighting.health}/{action.fighting.maxHealth}]";

            await ReplyAsync(outp);

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
            // Save everything
            await DatabaseHandler.SaveAllAsync();
        }

        [Command("players")]
        public async Task ListPlayers()
        {
            // Create a stringbuilder for all players
            StringBuilder outp = new StringBuilder("Players: \n");

            // Loop through all players
            foreach (Player p in PlayerHandler.GetPlayers())
            {
                // Append the player and their current action to the stringbuilder
                outp.Append(p.playerName + ": " + p.currentAction + "\n");
            }

            // Output the stringbuilder
            await ReplyAsync(outp.ToString());
        }

        [Command("reply")]
        public async Task Reply()
        {
            // Check if the player can log in
            if (!await PlayerHandler.AttemptLogin(Context.User))
            {
                // If they cannot login, notify them
                await ReplyAsync(Context.User.Username + ": " + Modules.NOT_REGISTERED_MSG);
                return;
            }

            Player p = PlayerHandler.GetPlayer(Context.User);
            string eq = p.equipment.ToString();
            await ReplyAsync(p.equipment.ToString());
        }

        [Command("give")]
        public async Task GiveCommand(string item, int count = 1)
        {
            // Check if the player can log in
            if (!await PlayerHandler.AttemptLogin(Context.User))
            {
                // If they cannot login, notify them
                await ReplyAsync(Context.User.Username + ": " + Modules.NOT_REGISTERED_MSG);
                return;
            }

            Player player = PlayerHandler.GetPlayer(Context.User);

            // Check if the given item exists
            if (!ItemHandler.IsRegisteredItem(item))
            {
                // If the given item does not exist, notify the player
                await ReplyAsync(Context.User.Username + ": Invalid item");
                return;
            }

            Item toAdd = ItemHandler.GetItemInstanceFromName(item);

            // Give the player the item
            player.inventory.AddItem(new ItemStack(toAdd, count));

            // Notify the player
            await ReplyAsync(Context.User.Username + ": Gave " + count + " of item " + toAdd.displayName);
        }

        [Command("dec")]
        public async Task DecrementCommand(int slot)
        {
            // Check if the player can log in
            if (!await PlayerHandler.AttemptLogin(Context.User))
            {
                // If they cannot login, notify them
                await ReplyAsync(Context.User.Username + ": " + Modules.NOT_REGISTERED_MSG);
                return;
            }

            Player p = PlayerHandler.GetPlayer(Context.User);

            if (!Enumerable.Range(1, p.inventory.size).Contains(slot))
            {
                await ReplyAsync($"{p.playerName}: That slot does not exist");
                return;
            }

            // Get the item to equip
            // The -1 is to have slots 1-28, rather than 0-27
            ItemStack itemToEquip = p.inventory[slot - 1];

            // Check if there is an item in the given slot
            if (itemToEquip == null || itemToEquip.IsEmpty)
            {
                await ReplyAsync(p.playerName + ": That slot is empty");
                return;
            }

            p.inventory[slot-1].count--;

        }

        [Command("lo")]
        public async Task LogoutCommand()
        {
            // Check if the player can log in
            if (!await PlayerHandler.AttemptLogin(Context.User))
            {
                // If they cannot login, notify them
                await ReplyAsync(Context.User.Username + ": " + Modules.NOT_REGISTERED_MSG);
                return;
            }

            // Remove the player instance
            PlayerHandler.RemovePlayerInstance(Context.User);
        }

        [Command("en")]
        public async Task EnemyInfoCommand()
        {
            // Check if the player can log in
            if (!await PlayerHandler.AttemptLogin(Context.User))
            {
                // If they cannot login, notify them
                await ReplyAsync(Context.User.Username + ": " + Modules.NOT_REGISTERED_MSG);
                return;
            }

            Player player = PlayerHandler.GetPlayer(Context.User);

            // Check if the player is fighting something
            if (player.currentAction is ActionFighting)
            {
                ActionFighting f = player.currentAction as ActionFighting;
                await ReplyAsync(f.ToString());
                return;
            }
            // If the player is not fighting anything, notify them
            await ReplyAsync(Context.User.Username + ": You are not fighting anything");
        }

        [Command("ser")]
        public async Task XmlCommand()
        {

            // The amount of objects
            int count = 100;

            string path = Environment.GetEnvironmentVariable("DISCORDMMO_USERDATA") + @"\Debug";

            // Notify the player
            await ReplyAsync("Serializing " + count + " ItemStacks");

            // Start a stopwatch
            Stopwatch w = Stopwatch.StartNew();


            for (int i = 0; i < count; i++)
            {
                try
                {
                    string itemPath = path + $"\\item{i}.xml";


                    // Serialize the object to a file
                    using (MemoryStream mem = new MemoryStream())
                    using (FileStream file = new FileStream(itemPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete | FileShare.ReadWrite, bufferSize: 1000000000, useAsync: true))
                    {

                        SerializationHandler.GetSerializer<ItemStack>().Serialize(mem, (ItemStack)ItemHandler.GetItemInstanceFromName("wood"));
                        byte[] b = mem.ToArray();
                        await file.WriteAsync(b, 0, b.Length);
                    }

                    // Recreate the item from the file
                    using (StreamReader file = new StreamReader(itemPath))
                    {
                        ItemStack item = (ItemStack)SerializationHandler.GetSerializer<ItemStack>().Deserialize(file);
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            // Stop the stopwatch
            w.Stop();

            // Notify the player with the stats
            await ReplyAsync("Serializing " + count + " objects took " + w.ElapsedMilliseconds + "ms \n" +
                             "Average time per object: " + w.ElapsedMilliseconds / count + "ms");

        }

        [Command("deluser")]
        public async Task DeleteUserCommand(string confirm = "no")
        {
            if (confirm == "yes")
            {
                await ReplyAsync(Context.User.Username + ": Attempting to delete account...");
                if (!await PlayerHandler.AttemptLogin(Context.User))
                {
                    await ReplyAsync(Context.User.Username + ": Cannot delete your account: You do not have an account");
                    return;
                }
                PlayerHandler.RemovePlayerInstance(Context.User);
                await DatabaseHandler.DeletePlayerAsync(Context.User);
                await ReplyAsync(Context.User.Username + ": Your account has successfully been deleted");
            }
            else
            {
                await ReplyAsync(Context.User.Username + ": It looks like you're trying to delete your account.\n" +
                    "If you are absolutely sure you would like to do this, use the command \"$$deluser yes\"\n" +
                    "WARNING: This action cannot be reversed");
            }
        }


    }


}
