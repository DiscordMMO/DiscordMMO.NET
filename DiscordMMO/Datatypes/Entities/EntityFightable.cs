using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using DiscordMMO.Util;
using DiscordMMO.Datatypes.Interactions;

namespace DiscordMMO.Datatypes.Entities
{
    [XmlRoot]
    public abstract class EntityFightable : Entity, IDamageable
    {

        [XmlElement]
        public abstract bool singleOnly { get; set; }

        [XmlIgnore]
        public Player fightingAgainst { get; set; }

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

        [XmlIgnore]
        public virtual OnBeforeAttacked BeforeAttackedEvent { get; set; }
        [XmlIgnore]
        public virtual OnBeforeAttacking BeforeAttackingEvent { get; set; }

        [XmlIgnore]
        public virtual OnAfterAttacked AfterAttackedEvent { get; set; }
        [XmlIgnore]
        public virtual OnAfterAttacking AfterAttackingEvent { get; set; }

        public virtual bool CanAttack(ref OnAttackEventArgs args) => true;

        public EntityFightable()
        {
            health = maxHealth;
            interactions.Add(new FightInteraction());
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
