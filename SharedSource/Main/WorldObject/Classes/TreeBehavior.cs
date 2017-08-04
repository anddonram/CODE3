using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace Codigo
{
    public class TreeBehavior:Behavior
    {
        [RequiredComponent]
        private WorldObject wo;

        private float growthSpeed=20;
        private double count=0;
        protected override void Update(TimeSpan gameTime)
        {
            count += gameTime.TotalSeconds;
            /**
             * automatic healing
             */
            if (wo.GetHealth() < wo.GetMaxHealth())
            {
                if (count >= growthSpeed)
                {
                    count = 0;
                    wo.TakeDamage(-1);
                }
            }else
            {
                /**
                 * the tree grows, increasing its max health
                 */
                wo.SetMaxHealth(wo.GetMaxHealth()+1);
                count = 0;

            }
        }
    }
}
