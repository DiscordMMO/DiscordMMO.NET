﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Util;

namespace DiscordMMO.Datatypes.Areas
{
    public class AreaWilderness : Area
    {
        public override string name { get; set; } = "wilderness";
        public override string displayName { get; set; } = "Wilderness";

        public override Direction blockedAt => Direction.NONE;

        public bool isUnloaded = true;

    }
}
