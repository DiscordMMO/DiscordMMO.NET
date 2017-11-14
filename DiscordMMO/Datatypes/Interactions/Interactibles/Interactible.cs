using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using System.Xml.Serialization;

namespace DiscordMMO.Datatypes.Interactions.Interactibles
{
    [XmlRoot]
    public abstract class Interactible
    {

        [XmlElement]
        public abstract string name { get; set; }

        [XmlIgnore]
        public abstract string displayName { get; set; }

        [XmlElement]
        public virtual List<Interaction> interactions { get; set; }

        public Interactible()
        {
            interactions = new List<Interaction>();
        }

        public virtual void Interact(int index, ref Player interactor, ICommandContext Context)
        {
            interactions[index].Interact(ref interactor, Context);
            
        }

    }
}
