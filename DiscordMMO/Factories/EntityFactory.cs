using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMMO.Datatypes.Entities;
using DiscordMMO.Datatypes.Interactions;

namespace DiscordMMO.Factories
{
    public static class EntityFactory
    {

        public static EntityFightable CreateFightable<T>() where T : EntityFightable, new()
        {
            EntityFightable fightable = new T();

            fightable.interactions.Add(new FightInteraction { owner = fightable });

            return fightable;

        }

    }
}
