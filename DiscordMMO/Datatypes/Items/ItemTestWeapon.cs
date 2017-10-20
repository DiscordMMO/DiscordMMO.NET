﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Util;
using DiscordMMO.Datatypes.Entities;

namespace DiscordMMO.Datatypes.Items
{
    public class ItemTestWeapon : ItemWeapon
    {
        public override string itemName => "test_weapon";

        public override string displayName => "Test Weapon";

        public override int attackRate => 6;

        public override int attackDamage => 5;

        public override int accuracy => 75;

        public override void OnEquip(Player player)
        {
            base.OnEquip(player);
            player.AfterAttackingEvent += AfterAttacking;
        }

        public override void OnUnEquip(Player player)
        {
            base.OnUnEquip(player);
            player.AfterAttackingEvent -= AfterAttacking;
        }

        public void AfterAttacking(ref OnAttackEventArgs args)
        {
            Random r = new Random();
            int triggerChance = 50;
            if (r.Next(100) >= triggerChance)
            {
                args.attacker.Attack(args.attacked, false);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(args.attacker.name + "'s swiftness has activated");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

    }
}
