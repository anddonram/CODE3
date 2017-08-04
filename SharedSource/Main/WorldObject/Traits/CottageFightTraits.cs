using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace Codigo.Components
{
    class CottageFightTraits:FightTraits
    {
        [RequiredComponent]
        private Cottage cottage;
        
        public override Boolean CanAttack()
        {
            return cottage.GetOccupation()!=0 && !cottage.UnderConstruction();
        }

        public override float GetAttack()
        {
            return 5*this.Owner.FindComponent<Cottage>().GetOccupation();
        }
    }
}
