using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Util;
using DiscordMMO.Handlers;
using System.Xml.Serialization;

namespace DiscordMMO.Datatypes.Preferences
{
    [HasOwnSerializer]
    [XmlRoot("Preferences")]
    public class PreferenceDictionary : SerializableDictionary<string, Preference>
    {
    }
}
