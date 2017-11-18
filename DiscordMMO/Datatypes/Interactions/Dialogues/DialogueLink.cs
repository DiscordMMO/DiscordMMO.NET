using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.Interactions.Dialogues
{
    public class DialogueLink
    {

        public int option;

        /// <summary>
        /// The node that this link leads to
        /// </summary>
        public DialogueNode to;

        public string displayName;

        public virtual bool Available(Player player) => true;

    }
}
