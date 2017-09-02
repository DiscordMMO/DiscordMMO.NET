using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Interactions;
using DiscordMMO.Util;
using DiscordMMO.Handlers;

namespace DiscordMMO.Datatypes.Entities
{
    public class EntityGoblin : EntityFightable
    {
        [Serialized(0)]
        public override string name => "goblin";

        public override bool singleOnly => true;

        public override int maxHealth => 10;

        [Serialized(2)]
        public override int health { get; set; }

        public override int defence => 1;

        public override int attackRate => 3;

        public override int attackDamage => 2;

        public override int accuracy => 50;

        [Serialized(1)]
        public override int ticksUntilNextAttack { get; set; } = 0;


        protected override List<Interaction> interactions { get; set; }

        public override event OnAttacked AttackedEvent;
        public override event OnAttacking AttackingEvent;

        public override List<ItemStack> drops
        {
            get
            {
                return (new ItemStack[] { ItemHandler.GetItemInstanceFromName("wood")}).ToList();
            }
        }

        public override void CallAttackedEvent(ref OnAttackEventArgs args)
        {
            AttackedEvent(ref args);
        }

        public override void CallAttackingEvent(ref OnAttackEventArgs args)
        {
            AttackingEvent(ref args);
        }

        public override void OnOpponentDied(List<ItemStack> drops)
        {
        }
    }
}
