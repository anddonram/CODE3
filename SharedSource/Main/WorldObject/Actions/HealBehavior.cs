using Codigo.Components;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.TiledMap;

namespace Codigo.Behaviors
{
    class HealBehavior : Behavior,ActionBehavior
    {
        private Boolean cooldownON;

        [RequiredComponent]
        private WorldObject wo;
        [RequiredComponent]
        private Person person;

        /**
         * <summary>
         * who is being healed
         * </summary>
         */
        private WorldObject healed; 

        protected override void Update(TimeSpan gameTime)
        {
            if (wo.GetAction() == ActionEnum.Heal)
            {
                Heal();
            }
        }

        /**
          * <summary>
          * Heals an adjacent ally
          * </summary>
          */
        private void Heal()
        {

            if (healed != null && !cooldownON)
            {
                if (!healed.IsDestroyed() && healed.GetAction() != ActionEnum.Inside && wo.IsAdjacent(healed) && healed.GetMaxHealth() > healed.GetHealth())
                {      
                    if (person != null)
                    {
                        healed.Heal(person.GetHeal());
                        person.GainHealPoints();
                    }
                    cooldownON = true;
                    WaveServices.TimerFactory.CreateTimer(TimeSpan.FromSeconds(person.GetHealSpeed()), () => { cooldownON = false; }).Looped = false;
                }
                else
                {
                    StopHealing();
                }
            }

        }

        public void SetHealed(WorldObject wo)
        {
            if (CanAct(wo))
            {
                Building b = wo.Owner.FindComponent<Building>(false);
                if (b == null || !b.UnderConstruction())
                {
                    healed = wo;
                    this.wo.SetAction(ActionEnum.Heal);
                }
                else
                {
                    healed = null;
                }
            }
        }
        /**
         * <summary>
         * Stops healing
         * </summary>
         */
        public void StopHealing()
        {
            healed = null;
            wo.Stop();
        }

        public bool CanAct(WorldObject other)
        {
            return other != null && 
                !other.IsDestroyed() && 
                this.wo.IsSameTeam(other.player) && 
                !this.wo.IsActionBlocking() && 
                other != this.wo && other.GetMaxHealth() > other.GetHealth();
        }

        public void Act(WorldObject other)
        {
            SetHealed(other);
        }
        public CommandEnum GetCommand()
        {
            return CommandEnum.Heal;
        }

        public bool CanShowButton(WorldObject otherWO)
        {
            return otherWO != null && 
                (otherWO.IsMobile() || otherWO.GetAction() != ActionEnum.Build) && 
                otherWO != wo && otherWO.IsSameTeam(wo.player) && 
                otherWO.GetMaxHealth() > otherWO.GetHealth();
        }

        public string GetCommandName(WorldObject otherWO)
        {
            return string.Format("{1} {0}", otherWO.GetWoName(), otherWO.IsMobile() ? "Heal" : "Repair");
        }
    }
}
