using Codigo.Behaviors;
using Codigo.Components;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace Codigo.Data
{
    public class FakeTreeData : WorldObjectData
    {
        public FakeTreeData()
        {
            woName = "FakeTree";
            traversable = true;
            mobile = false;
            maxHealth = 100;
            health = 1;
            sprite = WaveContent.Assets.bush_PNG;
            woodRequired = 150;
            isBuilding = true;
        }

        public override Entity AddComponents(Entity ent)
        {
            return ent.AddComponent(new Building())
                                .AddComponent(new BuildingFogAdjacents()); 
        }

        public override Entity AddTraits(Entity ent)
        {
            return ent.AddComponent(new FakeTreeTraits());
        }
    }

}
