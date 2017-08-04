using Codigo.Behaviors;
using Codigo.Components;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace Codigo.Data
{
    public class TrapData : WorldObjectData
    {
        public TrapData()
        {
            woName = "Trap";
            traversable = true;
            mobile = false;
            maxHealth = 50;
            health = 1;
            sprite = WaveContent.Assets.cross_png;
            woodRequired = 50;
            isBuilding = true;
        }

        public override Entity AddComponents(Entity ent)
        {
            return ent.AddComponent(new TrapBehavior())
                                 .AddComponent(new Building())
                                 .AddComponent(new BuildingFogAdjacents());
        }

        public override Entity AddTraits(Entity ent)
        {
            return ent.AddComponent(new TrapTraits());
        }
    }
}
