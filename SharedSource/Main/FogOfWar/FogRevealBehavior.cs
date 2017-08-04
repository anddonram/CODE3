using Codigo.Components;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;

namespace Codigo
{
    class FogRevealBehavior : Behavior
    {
        
        [RequiredComponent]
        protected WorldObject wo;

        /**
         * <remarks>
         * It is not compulsory to add this component, so cannot use RequiredComponent
         * </remarks>
         */
        private FogAdjacents adjacents;

        private FogOfWar fog;

        private Vector2 lastPosition=-Vector2.One;

        private UIBehavior ui;

        protected override void Initialize()
        {
            base.Initialize();
            fog = FogOfWar.fog;
            ui = UIBehavior.ui;
        }
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            adjacents = Owner.FindComponent<FogAdjacents>(false);
        }
        protected override void Update(TimeSpan gameTime)
        {
            if (fog != null&&adjacents!=null)
                if (wo.player != null && wo.IsSameTeam(ui.activePlayer))
                {
                    Vector2 currentPosition = wo.GetCenteredPosition();
                    //Check if the visibility has changed so it can update
                    bool visChanged = adjacents.VisibilityChanged();
                    
                    //We were not revealing this tiles before, so now we do (with updated visibility)
                    if (lastPosition == -Vector2.One)
                    {
                        fog.SetUp(currentPosition, adjacents.GetAdjacents());
                        lastPosition = currentPosition;

                        //We must set visibility here as false, as we already took them into account
                        visChanged = false;
                    }
                    if (currentPosition != lastPosition||visChanged)
                    {
                        if (visChanged)
                        {
                            fog.Erase(lastPosition, adjacents.LastAdjacents());
                            fog.SetUp(currentPosition, adjacents.GetAdjacents());
                        }
                        else
                        {
                            fog.TurnTiles(currentPosition, lastPosition, adjacents.GetAdjacents());
                        }
                        lastPosition =currentPosition;
                    }
                }
                else
                {
                    lastPosition = -Vector2.One;
                }
        }

        private bool removed = false;
        protected override void Removed()
        {
            if (!removed && fog != null && wo.player != null && wo.IsSameTeam(ui.activePlayer) && adjacents != null)
            {
                removed = true;
                fog.Erase(wo.GetCenteredPosition(), adjacents.GetAdjacents());
            }
            base.Removed();
        }
  
    
    }
}
