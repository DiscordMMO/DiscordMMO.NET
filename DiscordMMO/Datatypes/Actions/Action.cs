using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Preferences;
using System.Data.SqlTypes;

namespace DiscordMMO.Datatypes.Actions
{
    public abstract class Action
    {
        public const string DONE_IN_FORMAT = "You will be done in {0:dd\\.hh\\:mm\\:ss}";

        public Player performer { get; protected set; }
        public DateTime finishTime { get; protected set; }

        public abstract bool setFinishTime { get; }

        public abstract string name { get; }

        public Action(Player performer)
        {
            this.performer = performer;
        }

        public static Action GetActionFromName(string name, Player player)
        {
            switch (name.ToLower())
            {
                case "idle":
                    return new ActionIdle(player);
                case "chop_wood":
                    return new ActionChopWood(player);
                default: return null;
            }
        }

        public virtual async Task OnTick()
        {
            if (DateTime.Now > finishTime)
            {
                await Finish();
            }
        }

        protected async virtual Task Finish()
        {
            if ((performer.GetPreference<bool>("pm")))
            {
                var privateChannel = await performer.GetPrivateChannel();
                await privateChannel.SendMessageAsync(GetFinishedFormattingSecondPerson());
            }
            performer.Idle(false);
        }

        public virtual void SetFinishTime(DateTime time)
        {
            finishTime = time;
        }

        public abstract string GetStartedFormattingSecondPerson();

        public abstract string GetActiveFormattingSecondPerson();
        public abstract string GetActiveFormattingThridPerson(bool mention);

        public abstract string GetFinishedFormattingSecondPerson();
    }

    public class ActionIdle : Action
    {
        public override string name { get => "idle"; }

        public override bool setFinishTime => false;

        public ActionIdle(Player performer) : base(performer)
        {
            finishTime = DateTime.MaxValue.AddYears(-100);
        }

        public async override Task OnTick()
        {
        }

        public override void SetFinishTime(DateTime time)
        {
            return;
        }

        protected override Task Finish()
        {
            throw new InvalidOperationException("Cannot finish idle action");
        }

        public override string GetActiveFormattingSecondPerson()
        {
            return "You are currently idle";
        }

        public override string GetStartedFormattingSecondPerson() => "You have started idling";

        public override string GetActiveFormattingThridPerson(bool mention) => (mention ? performer.user.Mention : performer.playerName) + " is currently idle";


        public override string GetFinishedFormattingSecondPerson()
        {
            throw new InvalidOperationException("Cannot finish idle action");
        }


    }

}
