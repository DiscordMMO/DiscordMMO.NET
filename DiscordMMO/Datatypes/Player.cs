using System;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordMMO.Datatypes.Preferences;
using DiscordMMO.Datatypes.Actions;
using DiscordMMO.Datatypes.Inventories;
using DiscordMMO.Datatypes.Entities;
using DiscordMMO.Datatypes.Items;
using DiscordMMO.Util;
using DiscordMMO.Handlers;
using Action = DiscordMMO.Datatypes.Actions.Action;

namespace DiscordMMO.Datatypes
{
    [XmlRoot]
    [HasOwnSerializer]
    [AlsoRequires(typeof(Preference))]
    public class Player : IDamageable
    {
        #region Fields / Properties

        public static XmlSerializer serializer;

        [XmlIgnore]
        /// <summary>
        /// The IUser that owns this player
        /// </summary>
        public IUser user { get; protected set; }

        [XmlElement]
        /// <summary>
        /// The <see cref="user"/>s id 
        /// </summary>
        public ulong ID;


        [XmlElement]
        /// <summary>
        /// The username of the player
        /// </summary>
        public string playerName;
        
        [XmlElement]
        /// <summary>
        /// The players inventory
        /// </summary>
        public PlayerInventory inventory;
        
        [XmlElement]
        /// <summary>
        /// The players equipment
        /// </summary>
        public PlayerEquimentInventory equipment;

        [XmlElement]
        /// <summary>
        /// The action the player is currently performing
        /// </summary>
        public Action currentAction { get; set; }

        [XmlIgnore]
        /// <summary>
        /// The players preferences
        /// </summary>
        protected Dictionary<string, Preference> preferences = new Dictionary<string, Preference>();

        [XmlArray]
        public List<(string key, Preference value)> Preferences
        {
            get
            {
                return preferences.Select(x => (x.Key, x.Value)).ToList();
            }
            set
            {
                preferences = value.ToDictionary(x => x.key, x => x.value);
            }
        }
            



        [XmlElement]
        /// <summary>
        /// <c>False</c> if the player can be attacked by multiple enemies at once
        /// </summary>
        public bool inSingleCombat = false;

        [XmlElement]
        /// <summary>
        /// The enemies that are targeting the player
        /// </summary>
        public List<EntityFightable> targetedBy = new List<EntityFightable>();


        public bool CanStartFight
        {
            get
            {
                return currentAction is ActionFighting;
            }
        }

        public bool IsIdle
        {
            get
            {
                return currentAction is ActionIdle;
            }
        }

#endregion

        #region IDamageable

        public event OnBeforeAttacked BeforeAttackedEvent;
        public event OnBeforeAttacking BeforeAttackingEvent;

        public event OnAfterAttacked AfterAttackedEvent;
        public event OnAfterAttacking AfterAttackingEvent;

        public int maxHealth => 30;

        // You can't do { get; set; } = maxHealth
        public int health { get; set; } = 30;

        public int defence => 0;

        public int attackDamage
        {
            get
            {
                if (equipment[PlayerEquipmentSlot.MAIN_HAND].IsEmpty)
                {
                    return 1;
                }
                return ((ItemWeapon)equipment[PlayerEquipmentSlot.MAIN_HAND].itemType).attackDamage;
            }
        }


        public int accuracy
        {
            get
            {
                if (equipment[PlayerEquipmentSlot.MAIN_HAND].IsEmpty)
                {
                    return 50;
                }
                return ((ItemWeapon)equipment[PlayerEquipmentSlot.MAIN_HAND].itemType).accuracy;
            }
        }


        public int attackRate 
        {
            get
            {
                if (equipment[PlayerEquipmentSlot.MAIN_HAND].IsEmpty)
                {
                    return 3;
                }
                return ((ItemWeapon)equipment[PlayerEquipmentSlot.MAIN_HAND].itemType).attackRate;
            }
        }
        
        public int ticksUntilNextAttack { get; set; }

        string IDamageable.name => playerName;

        [XmlIgnore]
        public List<ItemStack> drops => inventory.items.Concat(equipment.items).ToList();

        public string convertPrefix => "player";

        #endregion

        #region Constructors

        protected Player()
        {
            preferences["pm"] = Preference.GetPreference(false);
            preferences["mention"] = Preference.GetPreference(false);
        }

        public Player(IUser user) : this(user, user.Username)
        {

        }

        public Player(IUser user, string name) : this()
        {
            currentAction = new ActionIdle(this);
            this.user = user;
            ID = user.Id;
            playerName = name;
            inventory = new PlayerInventory(this);
            equipment = new PlayerEquimentInventory(this);
            preferences["pm"] = Preference.GetPreference(true);
            preferences["mention"] = Preference.GetPreference(true);
        }

        public Player(IUser user, string name, Action action) : this()
        {
            this.user = user;
            ID = user.Id;
            playerName = name;
            currentAction = action;
            inventory = new PlayerInventory(this);
            BeforeAttackedEvent += Player_AttackedEvent;
            BeforeAttackingEvent += Player_AttackingEvent;
        }

        private void Player_AttackingEvent(ref OnAttackEventArgs args)
        {
            var pm = GetPrivateChannel().GetAwaiter().GetResult();
            pm.SendMessageAsync("Attacking " + args.attacked).GetAwaiter().GetResult();
        }

        private void Player_AttackedEvent(ref OnAttackEventArgs args)
        {
            var pm = GetPrivateChannel().GetAwaiter().GetResult();
            pm.SendMessageAsync("Attacked by " + args.attacked).GetAwaiter().GetResult();
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

        public void PostConstructor(DiscordSocketClient client)
        {
            user = client.GetUser(ID);
        }

        #endregion

        #region Actions

        public virtual async Task Tick()
        {
            await currentAction.OnTick();
        }
        
        /// <summary>
        /// Makes the player idle
        /// </summary>
        /// <param name="announce">Whether it should be announced to the player that they started this action</param>
        /// <param name="force">Whether the action only should be started if the player is idle (It will always be started if <c>true</c>)</param>
        public virtual void Idle(bool announce)
        {
            SetAction(new ActionIdle(this), announce, true);
        }

        /// <summary>
        /// Sets the action that the player is currently doing
        /// </summary>
        /// <param name="action">The action to start</param>
        /// <param name="announce">Whether it should be announced to the player that they started this action</param>
        /// <param name="force">Whether the action only should be started if the player is idle (It will always be started if <c>true</c>)</param>
        /// <returns><c>True</c> if the action of the player was changed, <c>false</c> otherwise</returns>
        public virtual bool SetAction(Action action, bool announce, bool force = false)
        {
            // If the action should be forced, always set the action
            if (force)
            {
                currentAction = action;
                return true;
            }
            else
            {
                // If it is not forced, set the action if the player is idle
                if (currentAction is ActionIdle)
                {
                    currentAction = action;
                    return true;
                }
            }
            return false;
        }

        

#endregion

        #region Combat

        /// <summary>
        /// Start fighting the given <see cref="EntityFightable"/>
        /// </summary>
        /// <param name="against">The <see cref="EntityFightable"/> to fight against</param>
        /// <param name="force">If <c>true</c> the fight will only be started if both the player and <paramref name="against"/> can start fighting</param>
        /// <returns>If the fight was started, <c>true</c>, <c>false</c> otherwise</returns>
        /// <seealso cref="IDamageable"/>
        public virtual bool StartFight(EntityFightable against, bool force = false)
        {
            // If the fight is forced, start the fight
            if (force)
            {
                against.StartFightAgainst(this, true);
                SetAction(new ActionFighting(this, against), false);
                return true;
            }
            else
            {
                // Only start the fight, if both parts can start the fight
                if (CanStartFight && !IsIdle && against.CanStartFight)
                {
                    SetAction(new ActionFighting(this, against), false);
                    against.StartFightAgainst(this, false);
                    return true;
                }
                return false;
            }
        }

        public void CallBeforeAttackedEvent(ref OnAttackEventArgs args) => BeforeAttackedEvent?.Invoke(ref args);

        public void CallBeforeAttackingEvent(ref OnAttackEventArgs args) => BeforeAttackingEvent?.Invoke(ref args);

        public void CallAfterAttackedEvent(ref OnAttackEventArgs args) => AfterAttackedEvent?.Invoke(ref args);

        public void CallAfterAttackingEvent(ref OnAttackEventArgs args) => AfterAttackingEvent?.Invoke(ref args);

        public void Die(IDamageable killer)
        {
        
            // TODO: Handle death
            
            var pm = GetPrivateChannel().GetAwaiter().GetResult();

            pm.SendMessageAsync(killer.name + " killed you").Wait();
            
            
        }



        public void OnOpponentDied(List<ItemStack> drops)
        {
            // TODO: Implement loot from this
            IDMChannel pm = GetPrivateChannel().GetAwaiter().GetResult();
            pm.SendMessageAsync("You killed an enemy").GetAwaiter().GetResult();
        }

        #endregion

        #region Inventory
        /// <summary>
        /// Equip an item
        /// </summary>
        /// <param name="toEquip">The <see cref="ItemStack"/> to equip</param>
        public virtual void Equip(ItemStack toEquip)
        {
            if (toEquip.itemType is ItemEquipable == false)
            {
                throw new ArgumentException("Tried to equip an item that is not equipable. Use Player.Equip(ItemStack, PlayerEquipmentSlot) if this was intended");
            }
            PlayerEquipmentSlot slot = (toEquip.itemType as ItemEquipable).slot;
            Equip(toEquip, slot);
        }

        /// <summary>
        /// Equip an item
        /// </summary>
        /// <param name="toEquip">The <see cref="ItemStack"/> to equip</param>
        /// <param name="slot">The <see cref="PlayerEquipmentSlot"/> to equip the <see cref="ItemStack"/> to</param>
        public virtual void Equip(ItemStack toEquip, PlayerEquipmentSlot slot)
        {
            ItemStack currentEquip = equipment[slot];
            if (currentEquip.itemType.stackable && currentEquip.itemType.itemName == toEquip.itemType.itemName)
            {
                equipment[slot].count += toEquip.count;
                return;
            }
            equipment[slot] = toEquip;
            ItemEquipable oldEquip = currentEquip.itemType as ItemEquipable;
            ItemEquipable newEquip = toEquip.itemType as ItemEquipable;
            oldEquip?.OnUnEquip(this);
            newEquip?.OnEquip(this);
            inventory.AddItem(currentEquip);
        }

        #endregion

        #region Misc.
        public async Task<IDMChannel> GetPrivateChannel()
        {
            var channel = await user.GetOrCreateDMChannelAsync();
            return channel;
        }

        public T GetPreference<T>(string key)
        {
            return (T) preferences[key].value;
        }

        public void SetPreference(string key, object value)
        {
            preferences[key].value = value;
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

        public Dictionary<string, Preference> GetPreferences()
        {
            return preferences;
        }

        


        #endregion
    }
}
