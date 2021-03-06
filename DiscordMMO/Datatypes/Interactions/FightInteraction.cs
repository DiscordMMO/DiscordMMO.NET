﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Entities;
using Discord.Commands;
using DiscordMMO.Util;

namespace DiscordMMO.Datatypes.Interactions
{
    public class FightInteraction : Interaction
    {

        public override string name { get; set; } = "fight";
        public override string displayName { get; set; } = "Fight";

        public EntityFightable owner;

        public override void Interact(ref Player interactor, ICommandContext Context)
        {
            if (owner.CanStartFight && interactor.CanStartFight)
            {
                interactor.StartFight(owner, false);
                Context.Channel.SendMessage($"{Context.User.Username}: Started fighting {owner.displayName}");
            }
            else
            {
                Context.Channel.SendMessage($"{Context.User.Username}: You cannot fight that right now");
            }
        }

    }
}
