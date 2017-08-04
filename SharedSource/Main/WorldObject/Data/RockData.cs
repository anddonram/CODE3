using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace Codigo.Data
{
    public class RockData : WorldObjectData
    {
        public RockData()
        {
            woName = "Rock";
            traversable = false;
            mobile = false;
            maxHealth = 8000;
            health = 8000;
            sprite = WaveContent.Assets.rock_PNG;
            woodRequired = 0;
            isBuilding = false;
        }

        public override Entity AddComponents(Entity ent)
        {
            return ent;
        }
    }
}
