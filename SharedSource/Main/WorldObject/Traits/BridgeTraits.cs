using System;
using System.Collections.Generic;
using System.Text;

namespace Codigo.Components
{
    class BridgeTraits : WorldObjectTraits
    {
        public WorldObject water;
        public override bool IsTraversable(WorldObject other)
        {
            return wo.GetAction() != ActionEnum.Build ? base.IsTraversable(other) : false;
        }
    }
}
