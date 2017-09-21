﻿using System;
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
using ProtoBuf;
using Action = DiscordMMO.Datatypes.Actions.Action;

namespace DiscordMMO.Datatypes
{
    [ProtoContract]
    public class Player : IDamageable
    {

        /// <summary>
        /// The IUser that owns this player
        /// </summary>
        public IUser user { get; protected set; }

        [ProtoMember(0)]
        /// <summary>
        /// The <see cref="Player.user"/>s id 
        /// </summary>
        public ulong ID => user.Id;


        [ProtoMember(1)]
        /// <summary>
        /// The username of the player
        /// </summary>
        public readonly string playerName;

        Dick i have, vodka have for you, hack i do for you, who should i hack, i can get you money credit cards all type of accounts.
        my phone 28876424 call me
        
        
        [ProtoMember(2)]
        /// <summary>
        /// The players inventory
        /// </summary>
        public PlayerInventory inventory;
        
        [ProtoMember(3)]
        /// <summary>
        /// The players equipment
        /// </summary>
        public PlayerEquimentInventory equipment;

        [ProtoMember(4)]
        /// <summary>
        /// The action the player is currently performing
        /// </summary>
        public Action currentAction { get; protected set; }

        [ProtoMember(5)]
        /// <summary>
        /// The players preferences
        /// </summary>
        protected readonly Dictionary<string, IPreference> preferences = new Dictionary<string, IPreference>();

        [ProtoMember(7)]
        /// <summary>
        /// <c>False</c> if the player can be attacked by multiple enemies at once
        /// </summary>
        public bool inSingleCombat = false;

        [ProtoMember(6)]
        /// <summary>
        /// The enemies that are targeting the player
        /// </summary>
        public List<EntityFightable> targetedBy = new List<EntityFightable>();

        [ProtoMember(8)]
        /// <summary>
        /// The enemy that the player is fighting
        /// </summary>
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

        #region IDamageable

        public event OnAttacked AttackedEvent;
        public event OnAttacking AttackingEvent;

        public int maxHealth => 30;

        public int health { get; set; }

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

        public List<ItemStack> drops => inventory.items;

        public string convertPrefix => "player";

        #endregion

        #region Constructors
        public Player(IUser user) : this(user, user.Username)
        {

        }

        public Player(IUser user, string name)
        {
            currentAction = new ActionIdle(this);
            this.user = user;
            playerName = name;
            inventory = new PlayerInventory(this);
            equipment = new PlayerEquimentInventory(this);
            preferences["pm"] = (Preference<bool>)true;
            preferences["mention"] = (Preference<bool>)true;
        }

        public Player(IUser user, string name, Action action)
        {
            this.user = user;
            playerName = name;
            currentAction = action;
            inventory = new PlayerInventory(this);
            AttackedEvent += Player_AttackedEvent;
            AttackingEvent += Player_AttackingEvent;
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

        public void CallAttackedEvent(ref OnAttackEventArgs args) => AttackedEvent(ref args);

        public void Die(IDamageable killer)
        {
        
            // TODO: Handle death
            
            var pm = GetPrivateChannel().GetAwaiter().GetResult();

            pm.SendMessageAsync(killer.name + " killed you").Wait();
            
            
        }

        public void CallAttackingEvent(ref OnAttackEventArgs args) => AttackingEvent(ref args);


        public void OnOpponentDied(List<ItemStack> drops)
        {
            // TODO: Implement loot from this
            throw new NotImplementedException();
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
            equipment[slot] = toEquip;
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

        void IDamageable.CallAttackedEvent(ref OnAttackEventArgs args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
