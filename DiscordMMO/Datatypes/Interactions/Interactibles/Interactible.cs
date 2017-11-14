using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordMMO.Datatypes.Interactions.Interactibles
{
    public abstract class Interactible
    {

        public abstract string name { get; set; }
        public abstract string displayName { get; set; }


        public virtual List<Interaction> interactions { get; set; }

        public virtual void Interact(int index, ref Player interactor, ICommandContext Context)
        {
            interactions[index].Interact(ref interactor, Context);
            
        }

    }
}
