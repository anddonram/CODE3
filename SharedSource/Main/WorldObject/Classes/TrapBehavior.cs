using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace Codigo.Behaviors
{
    class TrapBehavior : Behavior
    {
        [RequiredComponent]
        private WorldObject wo;

        private double count=0;
        private float damageSpeed=2;

        protected override void Update(TimeSpan gameTime)
        {
            if (wo.GetAction() != ActionEnum.Build)
            {
                WorldObject enemy = Map.map.GetMobile(wo.GetX(), wo.GetY());
                if (enemy != null && !wo.IsSameTeam(enemy.player))
                {
                    count += gameTime.TotalSeconds;
                    if (count >= damageSpeed)
                    {
                        Owner.IsVisible = true;
                        enemy.TakeDamage(25);
                        wo.TakeDamage(5);
                        count = 0;
                    }
                }
                else { Owner.IsVisible = false; }
            }
        }
    }
}
