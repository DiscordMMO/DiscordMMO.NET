using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Interactions;
using DiscordMMO.Datatypes.Interactions.Interactibles;

namespace DiscordMMO.Datatypes.Entities
{
    public abstract class Entity : Interactible
    {

        public override List<Interaction> interactions { get; set; }

    }
}
