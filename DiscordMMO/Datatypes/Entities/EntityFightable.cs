using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using DiscordMMO.Util;

namespace DiscordMMO.Datatypes.Entities
{
    public abstract class EntityFightable : Entity, IDamageable, ISerializable
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

        public override abstract string name { get; }

        public abstract int maxHealth { get; }

        public abstract int health { get; set; }

        public abstract int defence { get; }

        public abstract int attackRate { get; }

        public abstract int attackDamage { get; }

        public abstract int accuracy { get; }

        public abstract int ticksUntilNextAttack { get; set; }

        public abstract List<ItemStack> drops { get; }

        public abstract event OnAttacked AttackedEvent;
        public abstract event OnAttacking AttackingEvent;

        public EntityFightable(SerializationInfo info, StreamingContext context)
        {
            health = info.GetInt32("health");
            ticksUntilNextAttack = info.GetInt32("attackDelay");
        }

        public abstract void CallAttackedEvent(ref OnAttackEventArgs args);

        public abstract void CallAttackingEvent(ref OnAttackEventArgs args);

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", name);
            info.AddValue("health", health);
            info.AddValue("attackDelay", ticksUntilNextAttack);
        }

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
    }
}
