using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordMMO.Util;

namespace DiscordMMO.Datatypes.Entities
{
    public delegate void OnBeforeAttacked(ref OnAttackEventArgs args, bool forced);
    public delegate void OnBeforeAttacking(ref OnAttackEventArgs args, bool forced);

    public delegate void OnAfterAttacked(ref OnAttackEventArgs args, bool forced);
    public delegate void OnAfterAttacking(ref OnAttackEventArgs args, bool forced);

    public interface IDamageable
    {

        event OnBeforeAttacked BeforeAttackedEvent;
        event OnBeforeAttacking BeforeAttackingEvent;

        event OnAfterAttacked AfterAttackedEvent;
        event OnAfterAttacking AfterAttackingEvent;

        string name { get; }

        int maxHealth { get; }

        int health { get; set; }

        int defence { get; }

        int attackDamage { get; }

        int accuracy { get; }

        int ticksUntilNextAttack { get; set; }

        List<ItemStack> drops { get; }

        /// <summary>
        /// The amount of ticks between each attack
        /// </summary>
        int attackRate { get; }

        bool CanAttack(ref OnAttackEventArgs args);

        void OnOpponentDied(List<ItemStack> drops);

        /// <summary>
        /// This is a hack, to be able to call the <see cref="BeforeAttackedEvent"/> from the extension methods
        /// </summary>
        void CallBeforeAttackedEvent(ref OnAttackEventArgs args, bool forced);

        /// <summary>
        /// This is a hack, to be able to call the <see cref="BeforeAttackingEvent"/> from the extension methods
        /// </summary>
        void CallBeforeAttackingEvent(ref OnAttackEventArgs args, bool forced);

        /// <summary>
        /// This is a hack, to be able to call the <see cref="AfterAttackedEvent"/> from the extension methods
        /// </summary>
        void CallAfterAttackedEvent(ref OnAttackEventArgs args, bool forced);

        /// <summary>
        /// This is a hack, to be able to call the <see cref="AfterAttackingEvent"/> from the extension methods
        /// </summary>
        void CallAfterAttackingEvent(ref OnAttackEventArgs args, bool forced);

    }

    public static class IDamageableHelper
    {


        /// <summary>
        /// The lowest max hit for an instance will be baseDamage / <see cref="MIN_DAMAGE_DIVISOR"/>
        /// </summary>
        public const int MIN_DAMAGE_DIVISOR = 10;

        #region Combat

        public static OnAttackEventArgs GetAttackingArgs(this IDamageable attacker, IDamageable target)
        {
            return new OnAttackEventArgs(attacker, target);
        }

        public static OnAttackEventArgs GetAttackedArgs(this IDamageable target, IDamageable attacker)
        {
            return new OnAttackEventArgs(attacker, target);
        }

        #region Attacked [DEPRECATED]

        /*
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

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attacked"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool Attacked(this IDamageable attacked, ref OnAttackEventArgs args)
        {
            attacked.CallBeforeAttackedEvent(ref args);

            int baseHit = args.damage;
            IDamageable attacker = args.attacker;

            attacked.CalculateMaxDamage(ref args);
            attacked.AttemptAttack(ref args);
            attacked.CalculateFinalDamage(ref args);

            attacked.health -= Math.Max(args.damage - attacked.defence, 0);
            if (attacked.health <= 0)
            {
                attacked.Die(attacker);
                return true;
            }

            return false;
        }

        */
        #endregion

        #region Damage calculation
        public static void CalculateMaxDamage(this IDamageable attacked, ref OnAttackEventArgs args)
        {
            args.damage = Math.Max(args.damage - args.attacked.defence, Math.Max(args.damage / MIN_DAMAGE_DIVISOR, 1));
        }

        public static void AttemptAttack(this IDamageable attacked, ref OnAttackEventArgs args)
        {
            lock (NumberUtil.randomLock)
            {
                int hit = NumberUtil.random.Next(100);
                if (hit > args.hitChance)
                {
                    args.damage = 0;
                }
            }
        }

        public static void CalculateFinalDamage(this IDamageable attacked, ref OnAttackEventArgs args)
        {
            lock (NumberUtil.randomLock)
            {
                if (args.damage <= 0)
                    return;

                args.damage = NumberUtil.random.Next(0, args.damage + 1);
            }
        }

        #endregion

        // TOOD: Add a proper death callback
        // TODO: Add loot
        public static async Task Die(this IDamageable damageable, IDamageable killer)
        {
            killer.OnOpponentDied(damageable.drops);
        }
        #region Attacking

        /// <summary>
        /// Attack <paramref name="target"/>
        /// </summary>
        /// <param name="attacker">The attacker</param>
        /// <param name="target">The target of the attack</param>
        /// <param name="triggersEffects">Does the attack trigger any special effects?</param>
        /// <returns><c>true</c> if <paramref name="target"/> died, <c>false</c> otherwise</returns>
        public static bool Attack(this IDamageable attacker, IDamageable target, bool triggersEffects = true)
        {
            OnAttackEventArgs args = GetAttackingArgs(attacker, target);
            args.triggersEffect = triggersEffects;
            return Attack(attacker, target, ref args);
        }

        /// <summary>
        /// Attack <paramref name="target"/>
        /// </summary>
        /// <param name="attacker">The attacker</param>
        /// <param name="target">The target of the attack</param>
        /// <param name="args">The parameters of the attack</param>
        /// <returns><c>true</c> if <paramref name="target"/> died, <c>false</c> otherwise</returns>
        public static bool Attack(this IDamageable attacker, IDamageable target, ref OnAttackEventArgs args)
        {

            if (!attacker.CanAttack(ref args))
                return false;

            attacker.CallBeforeAttackingEvent(ref args, !args.triggersEffect);
            target.CallBeforeAttackedEvent(ref args, !args.triggersEffect);

            if (args.cancelled)
                return false;

            int baseHit = args.damage;

            target.CalculateMaxDamage(ref args);
            target.AttemptAttack(ref args);
            target.CalculateFinalDamage(ref args);

            target.health -= Math.Max(args.damage - target.defence, 1);

            if (args.triggersEffect)
            {
                attacker.CallAfterAttackingEvent(ref args, !args.triggersEffect);
                attacker.CallAfterAttackedEvent(ref args, !args.triggersEffect);
            }

            if (target.health <= 0)
            {
                target.Die(attacker);
                return true;
            }

            return false;


        }

        #endregion
#endregion

        public static void Heal(this IDamageable target, int amount, float maxOverheal = 1)
        {
            float afterHeal = target.health + amount;
            target.health = (int)Math.Floor(afterHeal.Clamp(0f, target.maxHealth * maxOverheal));
        }


    }


}
