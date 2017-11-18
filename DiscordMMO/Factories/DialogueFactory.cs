using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Interactions.Dialogues;

namespace DiscordMMO.Factories
{
    public class DialogueFactory
    {

        List<DialogueNode> nodes = new List<DialogueNode>();

        public void AddNode(string id, string text, string npcName)
        {
            nodes.Add(new DialogueNode {id = id, text = text, npcName = npcName, hasPlayerResponse = false, playerResponse = "" });
        }

        public void AddNode(string id, string text, string npcName, bool hasPlayerResponse, string playerResponse, bool hasNpcResponse)
        {
            nodes.Add(new DialogueNode {id = id, text = text, npcName = npcName, hasPlayerResponse = hasPlayerResponse, playerResponse = playerResponse, hasNpcResonse = hasNpcResponse });
        }

        public void AddNode(DialogueNode node)
        {
            nodes.Add(node);
        }

        public void LinkNodes(string from, string to, string displayName)
        {
            DialogueNode fromNode = GetNode(from);
            DialogueNode toNode = GetNode(to);

            if (fromNode == null)
            {
                throw new ArgumentException("There is no node with the id " + from);
            }

            if (toNode == null)
            {
                throw new ArgumentException("There is no node with the id " + to);
            }

            DialogueLink link = new DialogueLink { to = toNode, displayName = displayName };

            fromNode.links.Add(link);

        }

        public void LinkNodes(string from, DialogueLink link)
        {
            DialogueNode fromNode = GetNode(from);

            if (fromNode == null)
            {
                throw new ArgumentException("There is no node with the id " + from);
            }

            fromNode.links.Add(link);

        }

        public DialogueNode GetDoneEntryNode(string entryNodeId)
        {
            DialogueNode ret = nodes.FirstOrDefault(x => x.id == entryNodeId);

            if (ret == null)
            {
                throw new ArgumentException("There is no node with the id " + entryNodeId);
            }

            return ret;

        }

        protected DialogueNode GetNode(string id) => nodes.FirstOrDefault(x => x.id == id);

    }
}
