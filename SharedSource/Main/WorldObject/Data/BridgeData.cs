using Codigo.Components;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace Codigo.Data
{
    public class BridgeData : WorldObjectData
    {
        public BridgeData()
        {
            woName = "Bridge";
            traversable = true;
            mobile = false;
            maxHealth = 100;
            health = 1;
            sprite = WaveContent.Assets.puente_png;
            woodRequired = 250;
            isBuilding = true;
        }

        public override Entity AddComponents(Entity ent)
        {
            return ent.AddComponent(new Building());
        }

        public override Entity AddTraits(Entity ent)
        {
            return ent.AddComponent(new BridgeTraits());
        }
    }
}
