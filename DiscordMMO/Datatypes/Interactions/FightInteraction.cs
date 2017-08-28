using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Entities;

namespace DiscordMMO.Datatypes.Interactions
{
    public class FightInteraction : Interaction
    {

        public EntityFightable owner;

        public override void Interact(Player interactor)
        {
            if (owner.CanStartFight && interactor.CanStartFight)
            {
                interactor.StartFight(owner, false);
            }
        }

    }
}
