﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordMMO.Util;
using DiscordMMO.Datatypes.Entities;

namespace DiscordMMO.Datatypes.Actions
{
    public class ActionFighting : Action
    {
        public override string name => "fight";

        public override bool setFinishTime => false;

        public IDamageable fighting;

        /// <summary>
        /// THIS CONSTRUCTOR IS ONLY USED TO REGISTER THIS ACTION DO NOT USE THIS UNDER ANY OTHER CIRCUMSTANCE
        /// </summary>
        /// <param name="performer"></param>
        public ActionFighting(Player performer) : base(performer) { }

        public ActionFighting(Player performer, IDamageable against) : base(performer)
        {
            fighting = against;
        }

        public override string GetActiveFormattingSecondPerson() => "You are fighting " + fighting.name;

        public override string GetActiveFormattingThridPerson(bool mention) => (mention ? performer.user.Mention : performer.playerName) + " is currently fighting " + fighting.name;

        public override string GetFinishedFormattingSecondPerson() => "You are done fighting " + fighting.name;

        public override string GetStartedFormattingSecondPerson() => "You have started fighting " + fighting.name;

        public async override Task OnTick()
        {
            performer.ticksUntilNextAttack--;
            fighting.ticksUntilNextAttack--;

            if (performer.ticksUntilNextAttack <= 0)
            {
                OnAttackEventArgs args = new OnAttackEventArgs(performer, fighting);
                performer.Attacking(ref args);
                if (fighting.Attacked(ref args))
                {
                    performer.Idle(false);
                    await Finish();
                    return;
                }
                performer.ticksUntilNextAttack = performer.attackRate;
            }
            if (fighting.ticksUntilNextAttack <= 0)
            {
                OnAttackEventArgs args = new OnAttackEventArgs(fighting, performer);
                fighting.Attacking(ref args);
                if (performer.Attacked(ref args))
                {
                    // TODO: Probably some death handling
                    await Finish();
                    return;
                }
                fighting.ticksUntilNextAttack = fighting.attackRate;
            }

        }

        public override string ToString()
        {
            return $"{name}.({fighting})";
        }


    }
}