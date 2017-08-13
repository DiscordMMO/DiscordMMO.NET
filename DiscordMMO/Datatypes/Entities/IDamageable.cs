using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.Entities
{
    public interface IDamageable
    {

        int maxHealth { get; }
        int health { get; set; }

        int defence { get; }

        int attackDamage { get; }

        int accuracy { get; }

    }

    public static class IDamageableHelper
    {

        // TODO: Add a proper damage/accuracy function
        public static void Damage(this IDamageable damageable, int baseHit, IDamageable attacker)
        {
            if (baseHit - damageable.defence >= 0)
                return;
            damageable.health -= baseHit - damageable.defence;
            if (damageable.health >= 0)
                damageable.Die(attacker);
        }

        // TOOD: Add a proper death callback
        // TODO: Add loot
        public static void Die(this IDamageable damageable, IDamageable killer)
        {
            Console.WriteLine(damageable + " killed by " + killer);
        }

    }


}
