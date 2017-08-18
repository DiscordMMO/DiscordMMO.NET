using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordMMO.Datatypes.Preferences;
using DiscordMMO.Datatypes.Actions;
using DiscordMMO.Datatypes.Inventories;
using DiscordMMO.Datatypes.Entities;
using Action = DiscordMMO.Datatypes.Actions.Action;

namespace DiscordMMO.Datatypes
{
    public class Player : IDamageable
    {

        public IUser user { get; protected set; }
        public readonly string name;

        public PlayerInventory inventory;
        public PlayerEquimentInventory equipment;

        public Action currentAction { get; protected set; }

        protected readonly Dictionary<string, IPreference> preferences = new Dictionary<string, IPreference>();

        public bool inSingleCombat = false;

        public List<EntityFightable> targetedBy = new List<EntityFightable>();

        public EntityFightable target { get; protected set; }

        public bool CanStartFight
        {
            get
            {
                if (!inSingleCombat)
                    return true;
                return (targetedBy.Count <= 0 && target == null);
            }
        }

        public bool IsIdle
        {
            get
            {
                return currentAction is ActionIdle;
            }
        }

        public int maxHealth => 30;

        public int health { get; set; }

        public int defence => 0;

        public int attackDamage => 4;

        public int accuracy => 1;

        public int attackRate => 3;

        #region Constructors
        public Player(IUser user) : this(user, user.Username)
        {

        }

        public Player(IUser user, string name)
        {
            currentAction = new ActionIdle(this);
            this.user = user;
            this.name = name;
            inventory = new PlayerInventory(this);
            preferences["pm"] = (Preference<bool>)true;
            preferences["mention"] = (Preference<bool>)true;
        }

        public Player(IUser user, string name, Action action)
        {
            this.user = user;
            this.name = name;
            currentAction = action;
            inventory = new PlayerInventory(this);
        }

        public Player(ulong id, DiscordSocketClient client) : this(id, client, client.GetUser(id).Username)
        {
            
        }

        public Player(ulong id, DiscordSocketClient client, string name) : this(client.GetUser(id), name)
        {

        }

        public Player(ulong id, DiscordSocketClient client, string name, Action action) : this(client.GetUser(id), name, action)
        {

        }

#endregion

        public async Task Tick()
        {
            await currentAction.OnTick();
        }
        /// <summary>
        /// Makes the player idle
        /// </summary>
        /// <param name="announce">Whether it should be announced to the player that they started this action</param>
        /// <param name="force">Whether the action only should be started if the player is idle (It will always be started if true)</param>
        public bool Idle(bool announce, bool force)
        {
            return SetAction(new ActionIdle(this), announce, force);
        }

        /// <summary>
        /// Sets the action that the player is currently doing
        /// </summary>
        /// <param name="action">The action to start</param>
        /// <param name="announce">Whether it should be announced to the player that they started this action</param>
        /// <param name="force">Whether the action only should be started if the player is idle (It will always be started if true)</param>
        /// <returns>True if the action of the player was changed, false otherwise</returns>
        public bool SetAction(Action action, bool announce, bool force)
        {
            if (force)
            {
                currentAction = action;
                return true;
            }
            else
            {
                if (currentAction is ActionIdle)
                {
                    currentAction = action;
                    return true;
                }
            }
            return false;
        }

        public bool StartFight(EntityFightable against, bool force)
        {
            if (force)
            {
                against.StartFightAgainst(this, true);
                return true;
            }
            else
            {
                if (CanStartFight)
                {
                    return against.StartFightAgainst(this, false);
                }
                return false;
            }
        }

        public async Task<IDMChannel> GetPrivateChannel()
        {
            var channel = await user.GetOrCreateDMChannelAsync();
            return channel;
        }

        public T GetPreference<T>(string key)
        {
            return preferences[key] as Preference<T>;
        }

        public void SetPreference<T>(string key, T value)
        {
            preferences[key] = (Preference<T>)value;
        }

        public void SetPreferenceWithType(string key, object value, Type type)
        {
            if (!type.IsAssignableFrom(value.GetType()))
            {
                throw new ArgumentException("Value is not of given type");
            }
            MethodInfo method = typeof(Player).GetMethod("SetPreference");
            method = method.MakeGenericMethod(type);
            method.Invoke(this, new Object[] { key, value});
        }

        public Dictionary<string, IPreference> GetPreferences()
        {
            return preferences;
        }

    }
}
