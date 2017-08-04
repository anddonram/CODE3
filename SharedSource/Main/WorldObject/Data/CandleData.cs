using Codigo.Components;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace Codigo.Data
{
    public class CandleData : WorldObjectData
    {
        public CandleData()
        {
            woName = "Candle";
            traversable = true;
            mobile = false;
            maxHealth = 30;
            health = 1;
            sprite = WaveContent.Assets.candle_png;
            woodRequired = 100;
            isBuilding = true;
        }

        public override Entity AddComponents(Entity ent)
        {
            return ent.AddComponent(new Building())
                              .AddComponent(new CandleFogAdjacents())
                              .AddComponent(new CandleBehavior());
        }
    }
}
