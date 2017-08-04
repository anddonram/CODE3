using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WaveEngine.Components.Animation;
using WaveEngine.Framework;

namespace Codigo.Behaviors
{
    /**
     * <summary>
     * This class handles the animation for a sub sprite of a wo, like the person t-shirt
     * </summary>
     */
     [Obsolete]
    class ChildBehavior:Behavior
    {
        [RequiredComponent]
        private Animation2D anim;

        private WorldObject wo;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            wo = Owner.Parent.FindComponent<WorldObject>();
        }

        protected override void Update(TimeSpan gameTime)
        {
            //Trace.WriteLine(wo.GetAction());
            if (wo.GetAction() == ActionEnum.Idle || wo.GetAction() == ActionEnum.Inside)
            {
                if (anim.State == WaveEngine.Framework.Animation.AnimationState.Playing)
                    anim.StopAnimation();
            }
            else
            {
                if (anim.State == WaveEngine.Framework.Animation.AnimationState.Stopped)
                    anim.PlayAnimation("Moving", true);
            }
            if ((wo.GetAction() == ActionEnum.Inside || (wo.IsMobile() && !FogOfWar.fog.IsVisible(wo.GetX(), wo.GetY()))) && Owner.IsVisible)
            {
                Owner.IsVisible = false;
            }
            else if (wo.GetAction() != ActionEnum.Inside && FogOfWar.fog.IsVisible(wo.GetX(), wo.GetY()) && wo.IsVisible(UIBehavior.ui.activePlayer))
            {
                Owner.IsVisible = true;
            }
        }
    }
}
