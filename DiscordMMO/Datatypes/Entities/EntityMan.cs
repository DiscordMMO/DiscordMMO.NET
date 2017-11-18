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

            // TOOD: Make this not terrible

            factory.AddNode("entry", "Hello", "Man", true, "Hello", true);
            factory.AddNode(new DialogueNodeGiveItems { id = "freeStuffResponse", hasNpcResonse = true, hasPlayerResponse = true, items = { ItemHandler.GetItemInstanceFromName("wood") }, npcName = "Man", playerResponse = "free stuff pl0x", text = "k" });
            factory.AddNode("goodbyeResponse", "what is wrong with you", "Man", false, "Goodbye", true);

            factory.LinkNodes("entry", "freeStuffResponse", "free stuff pl0x");
            factory.LinkNodes("entry", "goodbyeResponse", "Goodbye");

            interactions.Add(new InteractionTalk { entryNode = factory.GetDoneEntryNode("entry") });
        }

        public override void OnOpponentDied(List<ItemStack> drops)
        {
        }
    }
}
