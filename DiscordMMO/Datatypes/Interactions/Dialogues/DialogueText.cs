using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.Interactions.Dialogues
{
    public class DialogueText
    {
        public string text;
        public DialogueType type;

        public DialogueText() { }
        public DialogueText(string text, DialogueType type)
        {
            this.text = text;
            this.type = type;
        }

    }
}
