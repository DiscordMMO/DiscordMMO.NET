using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.Entities
{
    public abstract class EntityFightable : Entity
    {

        public abstract bool singleOnly { get; }

        public Player fightingAgainst { get; protected set; }

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
