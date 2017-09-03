using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DiscordMMO.Handlers;
using DiscordMMO.Util;


namespace DiscordMMO.Datatypes.Inventories
{
    public class PlayerEquimentInventory : PlayerInventory
    {

        const string slotRegex = "s:\\d";

        public override int size => 10;

        Dictionary<string, int?> slots = new Dictionary<string, int?>();

        public PlayerEquimentInventory(Player owner) : base(owner)
        {
            foreach (PlayerEquipmentSlot slot in Enum.GetValues(typeof(PlayerEquipmentSlot)))
            {
                slots[slot.ToString().ToLowerInvariant()] = (int)slot;
            }
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
                this[name] = value;
            }
        }

        public ItemStack this[PlayerEquipmentSlot slot]
        {
            get
            {
                return this[(int)slot];
            }

            set
            {
                this[(int)slot] = value;
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

                ret[slot] = SerializationHandler.Deserialize(item) as ItemStack;

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

    public enum PlayerEquipmentSlot
    {
        HEAD, BACK, CHEST, AMMO, MAIN_HAND, LEGS, OFF_HAND, RIGHT_RING, FEET, LEFT_RING
    }

    public static class PlayerEquipmentSlotExtension
    {
        public static string GetDisplayName(this PlayerEquipmentSlot slot)
        {
            
            return (Char.ToUpperInvariant(slot.ToString()[0]) + slot.ToString().Substring(1).ToLowerInvariant()).Replace('_', ' ');
        }

    }

}
