using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Entities;

namespace DiscordMMO.Util
{

    public class OnAttackedEventArgs : EventArgs
    {
        public IDamageable attacker;
        public IDamageable attacked;

        public int fullDamage;

    }

}
