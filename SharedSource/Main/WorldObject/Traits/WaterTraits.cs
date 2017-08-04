using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace Codigo.Components
{
    class WaterTraits : WorldObjectTraits
    {
        public override bool IsAttackable(WorldObject wo)
        {
            return false;
        }
        public override bool IsSelectable(Player p)
        {
            return false;
        }
    }
}
