﻿using System;
using System.Collections.Generic;
using DiscordMMO.Handlers;
using DiscordMMO.Datatypes.Items;

namespace DiscordMMO.Datatypes
{
    public class ItemStack
    {
        public Item itemType;
        public int count;

        public bool IsEmpty => (itemType is ItemEmpty || count <= 0);

        public static ItemStack empty { get; private set; }


        public ItemStack(Item type, int count)
        {
            itemType = type;
            this.count = count;
        }

        public ItemStack(Item type) : this(type, 1)
        {

        }

        static ItemStack()
        {
            if (empty == null)
            {
                empty = new ItemStack(ItemHandler.GetItemInstanceFromName("empty"));
            }
        }

        public override string ToString()
        {
            if (IsEmpty)
                return "Empty";
            return itemType.displayName + $"({count})";
        }

    }
}
