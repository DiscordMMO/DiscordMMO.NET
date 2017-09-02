using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DiscordMMO.Handlers;
using DiscordMMO.Util;

namespace DiscordMMO.Datatypes.Entities
{
    public abstract class EntityFightable : Entity, IDamageable
    {

        public abstract bool singleOnly { get; }

        public Player fightingAgainst { get; protected set; }

        //TODO: Add fighting

        public bool CanStartFight
        {
            get
            {
                if (!singleOnly)
                    return true;
                if (fightingAgainst == null)
                    return true;
                return false;
            }
        }

        public abstract int maxHealth { get; }

        public abstract int health { get; set; }

        public abstract int defence { get; }

        public abstract int attackRate { get; }

        public abstract int attackDamage { get; }

        public abstract int accuracy { get; }

        public abstract int ticksUntilNextAttack { get; set; }

        public abstract List<ItemStack> drops { get; }

        public string convertPrefix => "dmg";

        public abstract event OnAttacked AttackedEvent;
        public abstract event OnAttacking AttackingEvent;

        public abstract void CallAttackedEvent(ref OnAttackEventArgs args);

        public abstract void CallAttackingEvent(ref OnAttackEventArgs args);

        public abstract void OnOpponentDied(List<ItemStack> drops);

        public bool StartFightAgainst(Player player, bool force)
        {
            if (force)
            {
                fightingAgainst = player;
                return true;
            }
            else
            {
                if (!CanStartFight)
                    return false;
                fightingAgainst = player;
                return true;
            }
        }

        public override string ToString()
        {
            return $"dmg:[{name},{ticksUntilNextAttack},{health}]";
        }

        public static EntityFightable FromString(string s)
        {

            return SerializationHandler.Deserialize(s) as EntityFightable;

        }
    }
}
