using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Threading.Tasks;
using DiscordMMO.Util;
using DiscordMMO.Datatypes.Entities;

namespace DiscordMMO.Datatypes.Actions
{
    public class ActionFighting : Action
    {
        public override string name => "fight";

        public override bool hasSetFinishTime => false;

        [XmlElement]
        public EntityFightable fighting;

        public ActionFighting(Player performer) : base(performer) { }

        public ActionFighting() : base() { }

        public ActionFighting(Player performer, EntityFightable against) : base(performer)
        {
            fighting = against;
            performer.ticksUntilNextAttack = performer.attackRate;
            fighting.ticksUntilNextAttack = performer.attackRate;
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

    }
}
