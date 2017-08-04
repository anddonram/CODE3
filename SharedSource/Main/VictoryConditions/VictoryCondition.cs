using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace Codigo.VictoryConditions
{
    public abstract class VictoryCondition:Component
    {
        protected UIBehavior uibehavior;
        public bool active = false;
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            uibehavior = Owner.FindComponent<UIBehavior>();
        }
        public virtual Player GetWinner()
        {
            foreach (Player p in uibehavior.GetPlayers())
            {
                if (PlayerMeetsCondition(p))
                    return p;
            }
            return null;
        }
        public abstract bool PlayerMeetsCondition(Player p);

        public abstract string GetName();
    }
}
