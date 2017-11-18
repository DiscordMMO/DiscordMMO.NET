using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using DiscordMMO.Util;

namespace DiscordMMO.Datatypes.Interactions.Dialogues
{
    public class DialogueNode
    {

        public string id;

        public string npcName;

        public string text;
        public string playerResponse;

        public bool hasPlayerResponse;
        public bool hasNpcResonse;

        public List<DialogueLink> links = new List<DialogueLink>();


        public DialogueNode() { }

        public DialogueNode(string text)
        {
            this.text = text;
        }

        public virtual void Execute(ref Player player, IMessageChannel channel)
        {
            string outp = "";

            if (hasPlayerResponse)
            {
                outp += $"{player.playerName}: {playerResponse}";
            }

            if (hasNpcResonse && hasPlayerResponse)
            {
                outp += "\n";
            }

            if (hasNpcResonse)
            {
                outp += $"{npcName}: {text}";
            }

            channel.SendMessage(outp);

            // If this is the last node in the tree
            if (GetAvailableLinks(player).Count() <= 0)
            {
                player.currentDialogue = null;
            }

        }

        public void ShowOptions(Player player, IMessageChannel channel)
        {
            StringBuilder outp = new StringBuilder($"{player.user.Username}: Replies:\n");

            int i = 1;

            foreach (DialogueLink link in GetAvailableLinks(player))
            {
                outp.AppendLine($"{i}. {link.displayName}");
                i++;
            }
            channel.SendMessage(outp.ToString());
        }

        public IEnumerable<DialogueLink> GetAvailableLinks(Player player)
        {
            foreach (DialogueLink link in links)
            {
                if (link.Available(player))
                    yield return link;
            }
        }

    }
}
