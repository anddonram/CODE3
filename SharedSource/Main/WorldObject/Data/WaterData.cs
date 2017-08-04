using Codigo.Components;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace Codigo.Data
{
    public class WaterData : WorldObjectData
    {
        public WaterData()
        {
            woName = "Water";
            traversable = false;
            mobile = false;
            maxHealth = 8000;
            health = 8000;
            sprite = WaveContent.Assets.rio_png;
            woodRequired = 0;
            isBuilding = false;
        }

        public override Entity AddComponents(Entity ent)
        {
            return ent;
        }

        public override Entity AddTraits(Entity ent)
        {
            return ent.AddComponent(new WaterTraits());
        }
    }
}
