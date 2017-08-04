using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace Codigo.Data
{
    public class CastleData : WorldObjectData
    {
        public CastleData()
        {
            woName = "Castle";
            traversable = true;
            mobile = false;
            maxHealth = 150;
            health = 150;
            sprite = WaveContent.Assets.castle_png;
            woodRequired = 0;
            isBuilding = true;
        }

        public override Entity AddComponents(Entity ent)
        {
            return ent;
        }
    }
}
