using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using DiscordMMO.Util;

namespace DiscordMMO.Datatypes.Entities
{
    [XmlRoot]
    public abstract class EntityFightable : Entity, IDamageable
    {

        [XmlElement]
        public abstract bool singleOnly { get; set; }

        [XmlIgnore]
        public Player fightingAgainst { get; set; }

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

        [XmlElement]
        public override abstract string name { get; set; }

        [XmlIgnore]
        public abstract int maxHealth { get; }

        [XmlElement]
        public abstract int health { get; set; }

        [XmlElement]
        public abstract int defence { get; set; }

        [XmlIgnore]
        public abstract int attackRate { get; }

        [XmlElement]
        public abstract int attackDamage { get; set; }

        [XmlElement]
        public abstract int accuracy { get; set; }

        [XmlElement]
        public abstract int ticksUntilNextAttack { get; set; }

        [XmlIgnore]
        public abstract List<ItemStack> drops { get; }

        public abstract event OnBeforeAttacked BeforeAttackedEvent;
        public abstract event OnBeforeAttacking BeforeAttackingEvent;

        public EntityFightable(SerializationInfo info, StreamingContext context)
        {
            health = info.GetInt32("health");
            ticksUntilNextAttack = info.GetInt32("attackDelay");
        }

        public EntityFightable()
        {

        }

        public abstract void CallBeforeAttackedEvent(ref OnAttackEventArgs args);

        public abstract void CallBeforeAttackingEvent(ref OnAttackEventArgs args);

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
