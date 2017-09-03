﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordMMO.Util;

namespace DiscordMMO.Datatypes.Entities
{
    public delegate void OnAttacked(ref OnAttackEventArgs args);
    public delegate void OnAttacking(ref OnAttackEventArgs args);

    public interface IDamageable 
    {

        event OnAttacked AttackedEvent;
        event OnAttacking AttackingEvent;

        [Serialized(0)]
        string name { get; }

        int maxHealth { get; }

        [Serialized(2)]
        int health { get; set; }

        int defence { get; }

        int attackDamage { get; }

        int accuracy { get; }

        [Serialized(1)]
        int ticksUntilNextAttack { get; set; }

        List<ItemStack> drops { get; }

        /// <summary>
        /// The amount of ticks between each attack
        /// </summary>
        int attackRate { get; }

        void OnOpponentDied(List<ItemStack> drops);

        /// <summary>
        /// This is a hack, to be able to call the <see cref="AttackedEvent"/> from the extension methods
        /// </summary>
        void CallAttackedEvent(ref OnAttackEventArgs args);

        /// <summary>
        /// This is a hack, to be able to call the <see cref="AttackingEvent"/> from the extension methods
        /// </summary>
        void CallAttackingEvent(ref OnAttackEventArgs args);

    }

    public static class IDamageableHelper
    {

        /// <summary>
        /// The lowest max hit for an instance will be baseDamage / <see cref="MIN_DAMAGE_DIVISOR"/>
        /// </summary>
        public const int MIN_DAMAGE_DIVISOR = 10;

        #region Attacked

        /// <summary>
        /// Called when this <see cref="IDamageable"/> is attacked
        /// </summary>
        /// <param name="damageable">The <see cref="IDamageable"/> that is attacked</param>
        /// <param name="baseHit">The base max hit</param>
        /// <param name="attacker">The <see cref="IDamageable"/> that is attacking</param>
        /// <returns><code>true</code> if the attacked died.</returns>
        public static bool Attacked(this IDamageable damageable, int baseHit, IDamageable attacker)
        {
            var args = new OnAttackEventArgs(attacker, damageable);
            return damageable.Attacked(ref args);
        }

        public static bool Attacked(this IDamageable attacked, ref OnAttackEventArgs args)
        {

            attacked.CallAttackedEvent(ref args);

            int baseHit = args.damage;
            IDamageable attacker = args.attacker;

            attacked.CalculateMaxDamage(ref args);
            attacked.AttemptAttack(ref args);
            attacked.CalculateFinalDamage(ref args);

            attacked.health -= baseHit - attacked.defence;
            if (attacked.health >= 0)
            {
                attacked.Die(attacker);
                return true;
            }

            return false;

        }

        public static void CalculateMaxDamage(this IDamageable attacked, ref OnAttackEventArgs args)
        {
            args.damage = Math.Max(args.damage - args.attacked.defence, args.damage / MIN_DAMAGE_DIVISOR);
        }

        public static void AttemptAttack(this IDamageable attacked, ref OnAttackEventArgs args)
        {
            Random r = new Random();
            if (r.Next(100) > args.hitChance)
            {
                args.damage = 0;
            }
        }

        public static void CalculateFinalDamage(this IDamageable attacked, ref OnAttackEventArgs args)
        {
            if (args.damage >= 0)
                return;
            Random r = new Random();
            args.damage = r.Next(0, args.damage);
        }

        // TOOD: Add a proper death callback
        // TODO: Add loot
        public static async Task Die(this IDamageable damageable, IDamageable killer)
        {
            killer.OnOpponentDied(damageable.drops);
        }
        #endregion

        #region Attacking

        public static void Attacking(this IDamageable attacker, ref OnAttackEventArgs args)
        {
            attacker.CallAttackingEvent(ref args);
        }

#endregion

    }


}
