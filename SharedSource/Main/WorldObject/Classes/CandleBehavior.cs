using System;
using WaveEngine.Framework;

namespace Codigo
{
    public class CandleBehavior : Behavior
    {
        [RequiredComponent]
        private WorldObject wo;
        
        private double counter = 0;
        protected override void Update(TimeSpan gameTime)
        {
            if (wo.GetAction() != ActionEnum.Build)
            {
                counter += gameTime.TotalSeconds;

                if (counter >= wo.genericSpeed)
                {
                    counter = 0;
                    wo.TakeDamage(1);
                }
            }
        }
    }
}