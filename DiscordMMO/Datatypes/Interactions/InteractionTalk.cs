using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordMMO.Datatypes.Interactions.Dialogues;
using DiscordMMO.Util;

namespace DiscordMMO.Datatypes.Interactions
{
    public class InteractionTalk : Interaction
    {

        public DialogueNode entryNode;

        public override string name { get; set; } = "talk";
        public override string displayName { get; set; } = "Talk";

        public override void Interact(ref Player interactor, ICommandContext Context)
        {
            Dialogue dia = new Dialogue { player = interactor, channel = interactor.GetPrivateChannel().GetAwaiter().GetResult(), currentNode = entryNode };

            interactor.currentDialogue = dia;

            entryNode.Execute(ref interactor, interactor.GetPrivateChannel().GetAwaiter().GetResult());
        }

    }
}
