using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMMO.Datatypes.Interactions
{
    public abstract class Interaction
    {

        public abstract string name { get; set; }
        public abstract string displayName { get; set; }

        public abstract void Interact(ref Player interactor);


    }
}
