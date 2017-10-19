using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using DiscordMMO.Handlers;
using DiscordMMO.Datatypes.Items;

namespace DiscordMMO.Datatypes.Actions
{
    public class ActionChopWood : Action
    {

        public override bool hasSetFinishTime => true;

        public ActionChopWood() : base()
        {

        }

        public ActionChopWood(Player performer) : base(performer)
        {
            finishTime = DateTime.Now.AddSeconds(10);
        }

        protected async override Task Finish()
        {
            if (performer.inventory.CanAdd(ItemHandler.GetItemInstanceFromName(ItemWood.name)))
            {
                if (!performer.inventory.AddItem(new ItemStack(ItemHandler.GetItemInstanceFromName(ItemWood.name))))
                {
                    if (performer.GetPreference<bool>("pm"))
                    {
                        IDMChannel dm = await performer.user.GetOrCreateDMChannelAsync();
                        await dm.SendMessageAsync("You chopped some wood, but you did not have space in your inventory, so you did not get it");
                    }
                }
            }
            await base.Finish();
        }

        public override string name => "chop_wood";

        public override string GetStartedFormattingSecondPerson() => "You have started chopping wood";

        public override string GetActiveFormattingSecondPerson() => "You are currently chopping wood.";
        public override string GetActiveFormattingThridPerson(bool mention) => (mention ? performer.user.Mention : performer.playerName) + " is currently chopping wood.";

        public override string GetFinishedFormattingSecondPerson() => "You are done chopping wood.";
    }
}
