using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Interactions;
using DiscordMMO.Util;
using DiscordMMO.Handlers;
using System.Runtime.Serialization;

namespace DiscordMMO.Datatypes.Entities
{
    public class EntityGoblin : EntityFightable
    {

        public override string name { get; set; } = "goblin";

        public override string displayName { get; set; } = "Goblin";

        public override bool singleOnly { get; set; } = true;

        public override int maxHealth => 10;

        public override int health { get; set; } 

        public override int defence { get; set; } = 1;

        public override int attackRate => 3;

        public override int attackDamage { get; set; } = 2;

        public override int accuracy { get; set; } = 50;

        public override int ticksUntilNextAttack { get; set; } = 0;


        public override List<Interaction> interactions { get; set; }

        public override event OnBeforeAttacked BeforeAttackedEvent;
        public override event OnBeforeAttacking BeforeAttackingEvent;

        public override event OnAfterAttacked AfterAttackedEvent;
        public override event OnAfterAttacking AfterAttackingEvent;

        public override List<ItemStack> drops
        {
            get
            {
                return (new ItemStack[] { ItemHandler.GetItemInstanceFromName("wood")}).ToList();
            }
        }

        public EntityGoblin() : base()
        {

        }

        public override void OnOpponentDied(List<ItemStack> drops)
        {
        }
    }
}
