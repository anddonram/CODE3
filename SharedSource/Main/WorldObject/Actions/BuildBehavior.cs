using Codigo.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace Codigo.Behaviors
{
    class BuildBehavior : Behavior,ActionBehavior
    {

        [RequiredComponent]
        private WorldObject wo;                // This object's world object
        [RequiredComponent]
        private Person person;          //You know, someone has to build that

        private Building building = null;                          // The building that this person is building
        private Boolean cooldownON;                                 // It shows if it can build or it has to wait
   
        protected override void Update(TimeSpan gameTime)
        {
            if (wo.GetAction() == ActionEnum.Build)             //If this person is not building anymore
            {
                Build();
            }
        }
        /**
         * <summary>
         * Checks if this person can build and it builds the selected building (if any)
         * </summary>
         */
        private void Build()
        {
            if (building != null&&!building.wo.IsDestroyed())
            {
                if (!cooldownON) // If it is building something and it can build
                {
                    Boolean ended = building.Build(person.GetBuild());
                    if (ended)                                                             // If the building is complete
                    {
                        person.GainBuildPoints();
                        StopBuilding();
                    }
                    cooldownON = true;
                    WaveServices.TimerFactory.CreateTimer(TimeSpan.FromSeconds(person.GetBuildSpeed()), () => { cooldownON = false; }).Looped = false;
                }
            }else
            {
                StopBuilding();
            }
        }
        /**
         * <summary>
         * Marks a building as selected to build by this person
         * </summary>
         */
        public void SetBuilding(WorldObject b)
        {
            if (CanAct(b))
            {
                building = b.Owner.FindComponent<Building>(isExactType: false);
                if (building != null && building.UnderConstruction())
                {
                    this.wo.SetAction(ActionEnum.Build);
                }
                else
                {
                    building = null;
                }
            }
        }
        /**
         * <summary>
         * Stops building
         * </summary>
         */
        private void StopBuilding()
        {
            building = null;
            wo.Stop();
        }

        public bool CanAct(WorldObject other)
        {
            return wo.IsAdjacent(other) && 
                !other.IsDestroyed() && 
                wo.IsSameTeam(other.player) && 
                !wo.IsActionBlocking();
        }

        public void Act(WorldObject other)
        {
            SetBuilding(other);
        }

        public CommandEnum GetCommand()
        {
            return CommandEnum.Build;
        }

        public bool CanShowButton(WorldObject otherWO)
        {
            return otherWO != null && 
                !otherWO.IsMobile() && 
                otherWO.GetAction() == ActionEnum.Build && 
                otherWO.IsSameTeam(wo.player);
        }

        public string GetCommandName(WorldObject otherWO)
        {
            return "Build";
        }
    }
}
