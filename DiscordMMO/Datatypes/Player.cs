﻿using Discord;
using Discord.WebSocket;
using DiscordMMO.Datatypes.Actions;
using DiscordMMO.Datatypes.Entities;
using DiscordMMO.Datatypes.Inventories;
using DiscordMMO.Datatypes.Items.Equipable;
using DiscordMMO.Datatypes.Items.Equipable.Weapons;
using DiscordMMO.Datatypes.Interactions.Dialogues;
using DiscordMMO.Datatypes.Preferences;
using DiscordMMO.Handlers;
using DiscordMMO.Util;
using DiscordMMO.Datatypes.Areas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Drawing;
using DiscordMMO.Helpers;
using Action = DiscordMMO.Datatypes.Actions.Action;
using Direction = DiscordMMO.Util.Direction;

namespace DiscordMMO.Datatypes
{
    [XmlRoot]
    [HasOwnSerializer]
    [AlsoRequires(typeof(Preference))]
    public class Player : IDamageable
    {

        // TODO: Add skills?

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
        public PreferenceDictionary preferences = new PreferenceDictionary();

        [XmlElement]
        /// <summary>
        /// The X position of the player
        /// </summary>
        public int x = 0;

        [XmlElement]
        /// <summary>
        /// The Y position of the player
        /// </summary>
        public int y = 0;

        [XmlIgnore]
        /// <summary>
        /// A wrapper for <c>new Point(x, y)</c>
        /// </summary>
        public Point position
        {
            get
            {
                return new Point(x, y);
            }
            set
            {
                x = value.X;
                y = value.Y;
            }
        }

        [XmlIgnore]
        /// <summary>
        /// The area that the player is currently in
        /// </summary>
        public Area currentArea
        {
            get
            {
                return AreaHandler.GetArea(position);
            }
            set
            {
                position = value.position;
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

        [XmlIgnore]
        /// <summary>
        /// The DateTime is the time that the ItemStack was last updated, this is used for making the ItemStack disappear after some time
        /// </summary>
        public InventoryLootPile lootPile = new InventoryLootPile();

        public bool CanStartFight
        {
            get
            {
                return currentAction is ActionIdle;
            }
        }

        /// <summary>
        /// Can the player start an action?
        /// </summary>
        public bool IsIdle
        {
            get
            {
                return currentAction is ActionIdle;
            }
        }

        [XmlElement]
        /// <summary>
        /// The dialgouge that the player is in
        /// </summary>
        public Dialogue currentDialogue;

        [XmlIgnore]
        /// <summary>
        /// The last time the player did something (The last time PingActivity() was called)
        /// </summary>
        public DateTime lastActive = DateTime.Now;

#endregion

        #region IDamageable

        [XmlIgnore]
        /// <summary>
        /// The event that is raised before an attack hits the player
        /// </summary>
        public OnBeforeAttacked BeforeAttackedEvent { get; set; }

        [XmlIgnore]
        /// <summary>
        /// The event that is raised before the player attacks something
        /// </summary>
        public OnBeforeAttacking BeforeAttackingEvent { get; set; }

        [XmlIgnore]
        /// <summary>
        /// The event that is raised after the player attacks something
        /// </summary>
        public OnAfterAttacking AfterAttackingEvent { get; set; }

        [XmlIgnore]
        /// <summary>
        /// The event that is raised after the player is attacked
        /// </summary>
        public OnAfterAttacked AfterAttackedEvent { get; set; }

        /// <summary>
        /// The maximum health of the player<br/>
        /// This should be a SOFT cap
        /// </summary>
        public int maxHealth => 30;

        
        /// <summary>
        /// The current health of the player
        /// </summary>
        public int health { get; set; } = 30; // You can't do { get; set; } = maxHealth

        /// <summary>
        /// The defence of the player
        /// </summary>
        public int defence => 0;

        /// <summary>
        /// The base attack damage of the player
        /// </summary>
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

        /// <summary>
        /// The accuracy of the player in percentage<br>
        /// If the target's defence is 0, this is just the chance in percentage to not hit 0
        /// </summary>
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

        /// <summary>
        /// The amount of ticks between each attack
        /// </summary>
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
        
        /// <summary>
        /// The amount of ticks until the next attack occurs
        /// </summary>
        public int ticksUntilNextAttack { get; set; }

        [XmlIgnore]
        /// <summary>
        /// The internal name of the player
        /// </summary>
        public string name => playerName;

        [XmlIgnore]
        /// <summary>
        /// The display name of the player
        /// </summary>
        public string displayName => playerName;

        [XmlIgnore]
        /// <summary>
        /// What the player will drop when they die
        /// </summary>
        public List<ItemStack> drops => inventory.items.Concat(equipment.items).ToList();

        public bool CanAttack(ref OnAttackEventArgs args)
        {
            foreach (PlayerEquipmentSlot slot in Enum.GetValues(typeof(PlayerEquipmentSlot)))
            {
                if (equipment[slot] == null || equipment[slot].IsEmpty)
                    continue;

                if (!(equipment[slot].itemType as ItemEquipable).CanAttack(ref args))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// The chance that the player loses ammo when attacking
        /// </summary>
        public float ammoLossChance = 50;
        
        [XmlElement]
        /// <summary>
        /// The maximum amount of mana that the player can have<br/>
        /// This should be a SOFT cap
        /// </summary>
        public int maxMana = 100;

        [XmlElement]
        /// <summary>
        /// The current amount of mana that the player has
        /// </summary>
        public int mana;

        /// <summary>
        /// When doing something that consumes mana, it will use <c>manaCost*manaUsageModifier</c> mana
        /// </summary>
        public float manaUsageModifier = 1;

        /// <summary>
        /// How much mana is regenerated each tick
        /// </summary>
        public int manaRegen = 10;

        #endregion

        #region Constructors

        protected Player()
        {

            /*
            preferences["pm"] = false;
            preferences["mention"] = false;
            */
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
            preferences["pm"] = true;
            preferences["mention"] = true;
            mana = maxMana;
        }

        public Player(IUser user, string name, Action action) : this()
        {
            this.user = user;
            ID = user.Id;
            playerName = name;
            currentAction = action;
            inventory = new PlayerInventory(this);
            mana = maxMana;
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

        /// <summary>
        /// This should be called when a tick occurs
        /// </summary>
        public virtual async Task Tick()
        {
            await CheckIdle();
            RegenMana();
            UpdateLootpile();

            await TickCurrentAction();
        }
        
        /// <summary>
        /// Makes the player idle
        /// </summary>
        /// <param name="announce">Whether it should be announced to the player that they started this action</param>
        public virtual void Idle(bool announce)
        {
            // Forcibly set the players action to an idle action
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
                // Change the current action to the new action
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
        
        /// <summary>
        /// Move in the given direction
        /// </summary>
        /// <param name="direction">The direction to move in</param>
        /// <param name="force">whether the movement should be forced or not</param>
        /// <returns><c>true</c> if the player was successfully moved, <c>false</c> otherwise</returns>
        public virtual (bool succes, string reason) Move(Direction direction, bool force = false)
        {
            if (!force)
            {

                if (!IsIdle)
                {
                    return (false, "You are already doing something");
                }

                if (!CanMoveInDirection(direction))
                {
                    return (false, "You cannot move in that direction");
                }
            }


            Point moveTo = new Point(x, y);

            moveTo.Offset(direction.GetOffset());

            Area area = AreaHandler.GetArea(moveTo);

            ActionMoving action = new ActionMoving();
            action.performer = this;
            action.SetFinishTime(DateTime.Now.AddSeconds(area.GetMoveTime(direction)));
            action.movingTo = area;

            return (SetAction(action, force), "You are already doing something");

        }

        public virtual bool CanMoveInDirection(Direction direction)
        {

            Point offset = direction.GetOffset();

            Point moveTo = new Point(x, y);
            moveTo.Offset(offset);

            Area area = AreaHandler.GetArea(moveTo);

            return !area.blockedAt.HasFlag(direction);

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

        public async Task Die(IDamageable killer)
        {

            currentArea.CreateGravestone(this);
            inventory.Clear();
            equipment.Clear();

            var pm = await GetPrivateChannel();

            await pm.SendMessageAsync(killer.displayName.CapitalizeFirst() + " killed you");
            
            
        }



        public void OnOpponentDied(List<ItemStack> drops)
        {
            foreach (ItemStack item in drops)
            {
                lootPile.Add(item);
            }
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
            oldEquip?.OnUnequip(this);
            newEquip?.OnEquip(this);
            inventory.AddItem(currentEquip);
            inventory.items.RemoveAt(inventory.items.FindIndex(x => x.itemType == toEquip.itemType));
        }

        /// <summary>
        /// Try to take an item from the loot pile and add it to the inventory
        /// </summary>
        /// <param name="index">The index of the item to loot</param>
        /// <returns><c>true</c> if the item was successfully added to the inventory, <c>false</c> otherwise</returns>
        public virtual (bool success, string errorReason) AttemptLoot(int index)
        {
            ItemStack toAdd = lootPile.ItemStacks[index];
            if (!inventory.CanAdd(toAdd))
                return (false, "You do not have enough inventory space to do that");
            lootPile.RemoveAt(index);
            inventory.AddItem(toAdd);
            return (true, "");
        }

        #endregion

        #region Ticking
        /// <summary>
        /// Check if the player has been idle for more than the idle time
        /// </summary>
        private async Task CheckIdle()
        {
            // Remove the player instance if the player has been inactive for more than Server.IDLE_TIME seconds
            if (lastActive.AddSeconds(Server.IDLE_TIME) <= DateTime.Now)
            {
                await DatabaseHandler.SaveAsync(this);
                var pm = await GetPrivateChannel();
                await MessageHandler.SendMessageAsync(pm, "You have been kicked for being idle for too long");

                PlayerHandler.RemovePlayerInstance(this);
            }
        }

        private void RegenMana()
        {
            mana = Math.Min(mana + manaRegen, maxMana);
        }

        private void UpdateLootpile()
        {
            lootPile.Update();
        }

        private void TickEquipped()
        {
            foreach (ItemEquipable item in equipment.items.Select(x => x.itemType as ItemEquipable))
            {
                if (item == null) continue;
                item.WhileEquipped(this);
            }
        }

        private async Task TickCurrentAction()
        {
            await currentAction.OnTick();
        }

        #endregion

        #region Misc.

        public void PingActivity()
        {
            lastActive = DateTime.Now;
        }

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

        public PreferenceDictionary GetPreferences()
        {
            return preferences;
        }

        


        #endregion
    }
}
