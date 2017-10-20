using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Entities;

namespace DiscordMMO.Util
{

    public class OnAttackEventArgs : EventArgs
    {
        public IDamageable attacker;
        public IDamageable attacked;


        public int hitChance;
        public int damage;

        public bool triggersEffect;

        public OnAttackEventArgs(IDamageable attacker, IDamageable attacked, bool effectsTrigger = true)
        {
            this.attacked = attacked;
            this.attacker = attacker;
            hitChance = attacker.accuracy;
            damage = attacker.attackDamage;
            triggersEffect = effectsTrigger;
        }

    }

}
