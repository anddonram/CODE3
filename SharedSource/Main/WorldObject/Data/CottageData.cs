using Codigo.Behaviors;
using Codigo.Components;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace Codigo.Data
{
    public class CottageData : WorldObjectData
    {
        public CottageData()
        {
            woName = "Cottage";
            traversable = false;
            mobile = false;
            maxHealth = 100;
            health = 1;
            sprite = WaveContent.Assets.cottage_png;
            woodRequired = 250;
            isBuilding = true;
        }

        public override Entity AddComponents(Entity ent)
        {
            return ent.AddComponent(new Cottage())
                                 .AddComponent(new CottageFightTraits())
                                 .AddComponent(new FightBehavior())
                                 .AddComponent(new CottageFogAdjacents());
        }
    }
}
