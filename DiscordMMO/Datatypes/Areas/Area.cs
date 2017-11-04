﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Entities;
using DiscordMMO.Util;

namespace DiscordMMO.Datatypes.Areas
{
    public class Area
    {

        public virtual string name { get; set; }

        public virtual Direction blockedAt { get; } = Direction.NONE;

        public List<Entity> content { get; protected set; }

    }
}