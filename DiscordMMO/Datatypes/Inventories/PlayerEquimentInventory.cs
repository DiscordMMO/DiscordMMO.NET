using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DiscordMMO.Datatypes.Inventories
{
    public class PlayerEquimentInventory : PlayerInventory
    {

        const string slotRegex = "s:\\d";

        public override int size => 10;

        Dictionary<string, int?> slots = new Dictionary<string, int?>();

        public PlayerEquimentInventory(Player owner) : base(owner)
        {

            slots["head"] = 0;
            slots["back"] = 1;
            slots["chest"] = 2;
            slots["ammo"] = 3;
            slots["main hand"] = 4;
            slots["legs"] = 5;
            slots["off hand"] = 6;
            slots["right hand"] = 7;
            slots["feet"] = 8;
            slots["left hand"] = 9;

        }


        public ItemStack this[string name]
        {
            get
            {
                if (GetIndexFromSlotName(name) == -1)
                    throw new ArgumentException("Invalid slot name");
                return this[GetIndexFromSlotName(name)];
            }
            set
            {
                if (GetIndexFromSlotName(name) == -1)
                    throw new ArgumentException("Invalid slot name");
                this[GetIndexFromSlotName(name)] = value;
            }
        }


        public int GetIndexFromSlotName(string name)
        {
            int? n = slots[name];
            return slots[name].GetValueOrDefault(-1);
        }

        public static new PlayerEquimentInventory FromString(Player owner, string inv)
        {

            PlayerEquimentInventory ret = new PlayerEquimentInventory(owner);
            for (int i = 0; i < inv.Split(';').Length - 1; i++)
            {
                string fullItem = inv.Split(';')[i];

                string item = fullItem.Split(',')[0];


                int slot = int.Parse(Regex.Matches(fullItem, slotRegex)[0].Value.Substring(2));

                ret[slot] = ItemStack.FromString(item);

            }
            return ret;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < items.Count; i++)
            {
                ItemStack item = items[i];
                string name = item.IsEmpty ? "empty" : item.itemType.itemName;
                b.Append($"{name},[s:{i}];");
            }
            string o = b.ToString();
            return o.Remove(o.Length - 1);
        }

    }
}
