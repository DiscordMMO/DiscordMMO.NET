using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Areas;

namespace DiscordMMO.Datatypes.Actions
{
    public class ActionMoving : Action
    {

        public ActionMoving() : base() { }

        public override bool hasSetFinishTime => true;

        public override string name => "moving";

        public Area movingTo;

        protected async override Task Finish()
        {
            performer.position = movingTo.position;
            await base.Finish();
        }

        public override string GetActiveFormattingSecondPerson() => $"You are moving to {movingTo.displayName}";

        public override string GetActiveFormattingThridPerson(bool mention) => $"{performer.user} is moving to {movingTo.displayName}";

        public override string GetFinishedFormattingSecondPerson() => $"You have reached {movingTo.displayName}";

        public override string GetStartedFormattingSecondPerson() => $"You have started moving towards {movingTo.displayName}";

    }
}
;