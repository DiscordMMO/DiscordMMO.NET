using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Factories;
using DiscordMMO.Datatypes.Interactions.Dialogues;
using DiscordMMO.Handlers;
using DiscordMMO.Datatypes.Interactions;

namespace DiscordMMO.Datatypes.Entities
{
    public class EntityMan : EntityFightable
    {

        public override bool singleOnly { get; set; } = true;
        public override string name { get; set; } = "man";

        public override int maxHealth => 5;

        public override int health { get; set; }
        public override int defence { get; set; } = 2;

        public override int attackRate => 3;

        public override int attackDamage { get; set; } = 2;
        public override int accuracy { get; set; } = 75;
        public override int ticksUntilNextAttack { get; set; } = 0;

        public override List<ItemStack> drops => new List<ItemStack>();

        public override string displayName { get; set; } = "Man";

        public EntityMan()
        {

            DialogueFactory factory = new DialogueFactory();

            factory.npcName = "Man";

            factory.AddNode("entry", "Hello", DialogueType.NPC_RESPONSE);
            factory.AddNode(new DialogueNodeGiveItems { id = "freeStuffResponse", items = { ItemHandler.GetItemInstanceFromName("wood") }}.AddText("free stuff pl0x", DialogueType.PLAYER_RESPONSE).AddText("k", DialogueType.NPC_RESPONSE).AddText("You got 1 piece of wood", DialogueType.NONE));
            factory.AddNode("goodbyeResponse", "what is wrong with you", DialogueType.NPC_RESPONSE);

            factory.LinkNodes("entry", "freeStuffResponse", "free stuff pl0x");
            factory.LinkNodes("entry", "goodbyeResponse", "Goodbye");

            interactions.Add(new InteractionTalk { entryNode = factory.GetDoneEntryNode("entry") });
        }

        public override void OnOpponentDied(List<ItemStack> drops)
        {
        }
    }
}
