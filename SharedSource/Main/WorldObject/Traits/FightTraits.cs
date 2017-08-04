using Codigo.Behaviors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace Codigo.Components
{
    class FightTraits : WorldObjectTraits
    {
        

        public virtual Boolean CanAttack()
        {
            return false;
        }
        
        public virtual float GetAttack()
        {
            return 0f;
        }
        public virtual float GetAttackSpeed()
        {
            return 2f;
        }
        public virtual void AfterAttack()
        {

        }

}
}
