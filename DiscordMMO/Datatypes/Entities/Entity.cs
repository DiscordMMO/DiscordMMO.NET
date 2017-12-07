using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DiscordMMO.Datatypes.Interactions;
using DiscordMMO.Datatypes.Interactions.Interactibles;

namespace DiscordMMO.Datatypes.Entities
{
    [XmlRoot]
    public abstract class Entity : Interactible
    {
        [XmlElement]
        public DateTime createdAt { get; set; } = DateTime.Now;

        [XmlIgnore]
        public override List<Interaction> interactions { get; set; }

    }
}
