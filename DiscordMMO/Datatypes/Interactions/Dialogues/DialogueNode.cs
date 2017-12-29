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

        public List<DialogueText> text = new List<DialogueText>();

        public List<DialogueLink> links = new List<DialogueLink>();


        public DialogueNode() { }

        public virtual void Execute(ref Player player, IMessageChannel channel)
        {
            string outp = "";

            foreach (DialogueText textPiece in text)
            {

                switch (textPiece.type)
                {
                    case DialogueType.NONE:
                        outp += textPiece.text;
                        break;
                    case DialogueType.NPC_RESPONSE:
                        outp += $"{npcName}: {textPiece.text}";
                        break;
                    case DialogueType.PLAYER_RESPONSE:
                        outp += $"{player.name}: {textPiece.text}";
                        break;
                    default:
                        outp += "Something went terribly wrong in the dialogue system! The default case was hit in DialogueNode.Execute";
                        break;
                }
                outp += "\n";
            }

            channel.SendMessage(outp);

            // If this is the last node in the tree
            if (GetAvailableLinks(player).Count() <= 0)
            {
                player.currentDialogue = null;
            }

            if (GetAvailableLinks(player).Count() == 1)
            {
                player.currentDialogue.SelectOption(0);
                return;
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

        public DialogueNode AddText(string text, DialogueType type)
        {
            return AddText(new DialogueText(text, type));
        }

        public DialogueNode AddText(DialogueText text)
        {
            this.text.Add(text);
            return this;
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
