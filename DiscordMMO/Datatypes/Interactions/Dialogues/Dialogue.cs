using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Xml.Serialization;

namespace DiscordMMO.Datatypes.Interactions.Dialogues
{
    [XmlRoot]
    public class Dialogue
    {

        [XmlElement]
        public DialogueNode currentNode;

        [XmlIgnore]
        public Player player;

        [XmlIgnore]
        public IMessageChannel channel;

        public void SelectOption(int index)
        {
            if (currentNode.GetAvailableLinks(player).ElementAtOrDefault(index) == null)
                return;
            currentNode = currentNode.GetAvailableLinks(player).ElementAt(index).to;
            currentNode.Execute(ref player, channel);
        }

    }
}
