﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.Interactions
{
    public class FightInteraction : Interaction
    {

        public EntityFightable owner;

        public override void Interact(Player interactor)
        {
            if (owner.CanStartFight && interactor.CanStartFight)
            {

            }
        }

    }
}
