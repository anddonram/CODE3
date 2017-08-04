using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace Codigo.Data
{
    public class TreeData : WorldObjectData
    {
        public TreeData()
        {
            woName = "Tree";
            traversable = false;
            mobile = false;
            health = 100;
            maxHealth = 100;
            sprite = WaveContent.Assets.bush_PNG;
            woodRequired = 0;
            isBuilding = false;
        }

        public override Entity AddComponents(Entity ent)
        {
          return ent.AddComponent(new TreeBehavior());
        }
    }
}
