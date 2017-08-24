using System;
using DiscordMMO.Util;

namespace DiscordMMO.Datatypes.Entities
{
    public delegate void OnAttacked(ref OnAttackedEventArgs args);

    public interface IDamageable
    {

        event OnAttacked AttackedEvent;

        int maxHealth { get; }
        int health { get; set; }

        int defence { get; }

        int attackDamage { get; }

        int accuracy { get; }

        /// <summary>
        /// The amount of ticks between each attack
        /// </summary>
        int attackRate { get; }

    }

    public static class IDamageableHelper
    {

        // TODO: Add a proper damage/accuracy function
        public static void Damage(this IDamageable damageable, int baseHit, IDamageable attacker)
        {
            damageable.Damage(new OnAttackedEventArgs { attacked = damageable, attacker = attacker, fullDamage = baseHit });
        }

        public static void Damage(this IDamageable attacked, OnAttackedEventArgs args)
        {

            // TODO: Figure out how to handle combat and events for IDamageable
            attacked.AttackedEvent(ref args);

            int baseHit = args.fullDamage;
            IDamageable attacker = args.attacker;


            if (baseHit - attacked.defence >= 0)
                return;
            attacked.health -= baseHit - attacked.defence;
            if (attacked.health >= 0)
                attacked.Die(attacker);

        }

        // TOOD: Add a proper death callback
        // TODO: Add loot
        public static void Die(this IDamageable damageable, IDamageable killer)
        {
            Console.WriteLine(damageable + " killed by " + killer);
        }

    }


}
