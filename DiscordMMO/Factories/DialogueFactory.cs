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

        public string npcName;

        public DialogueNode AddNode(string id, string text, DialogueType type)
        {
            DialogueNode node = new DialogueNode { id = id, npcName = npcName };
            node.text.Add(new DialogueText(text, type));
            nodes.Add(node);
            return node;
        }

        public DialogueNode AddNode(DialogueNode node)
        {
            node.npcName = npcName;
            nodes.Add(node);
            return node;
        }

        public DialogueNode AddText(string id, string text, DialogueType type)
        {
            return AddText(id, new DialogueText(text, type));
        }

        public DialogueNode AddText(string id, DialogueText text)
        {
            DialogueNode node = GetNode(id);
            return node.AddText(text);
        }

        /// <summary>
        /// Link two nodes
        /// </summary>
        /// <param name="from">The node the link goes from</param>
        /// <param name="to">The node the link goes to</param>
        /// <param name="displayName">The name of the option</param>
        /// <returns>The node that the link goes to</returns>
        public DialogueNode LinkNodes(string from, string to, string displayName)
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

            return toNode;

        }

        public DialogueNode LinkNodes(string from, DialogueLink link)
        {
            DialogueNode fromNode = GetNode(from);

            if (fromNode == null)
            {
                throw new ArgumentException("There is no node with the id " + from);
            }

            fromNode.links.Add(link);
            return link.to;
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
