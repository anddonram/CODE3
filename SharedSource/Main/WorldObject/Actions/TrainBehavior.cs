using Codigo.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace Codigo.Behaviors
{
    public class TrainBehavior : Behavior,ActionBehavior
    {
        private Boolean cooldownON;
        [RequiredComponent]
        private WorldObject wo;
        [RequiredComponent]
        private PersonFightTraits traits;

        /**
         * <summary>
         * The percentage under which any of the units will fall and stop the training session
         * </summary>
         */
        private float healthStop=0.2f;
        /**
         * <summary>
         * Who are we fighting with
         * </summary>
         */
        private TrainBehavior rival;

        protected override void Update(TimeSpan gameTime)
        {
            if (wo.GetAction() == ActionEnum.Train)
            {
                Train();
            }
        }

        /**
          * <summary>
          * Trains an adjacent partner
          * </summary>
          */
        private void Train()
        {
            if (traits.CanAttack()&& rival != null && rival.wo.GetAction() == ActionEnum.Train && rival.rival == this)
            {
                if ( !cooldownON)
                    if (!rival.wo.IsDestroyed() && wo.IsAdjacent(rival.wo)
                        && traits.GetAttack() < rival.wo.GetHealth() && rival.wo.GetHealth() >= rival.wo.GetMaxHealth() * healthStop)
                    {
                        rival.wo.TakeDamage(traits.GetAttack());
                        traits.AfterAttack();

                        cooldownON = true;
                        WaveServices.TimerFactory.CreateTimer(TimeSpan.FromSeconds(traits.GetAttackSpeed()), () => { cooldownON = false; }).Looped = false;
                    }
                    else
                    {
                        Stop();
                    }
            }
            else
            {
                Stop();
              
            }
        }

        private void Stop()
        {
            rival = null;
            wo.Stop();
        }

        /**
        * <summary>
        * starts training a person 
        * </summary>
        * <param name="riv">
        * the foe to be fought
        * </param>
        */
        public void SetRival(WorldObject riv)
        {

            if (CanAct(riv))
            { 
                TrainBehavior rivPerson = riv.Owner.FindComponent<TrainBehavior>();
                if (rivPerson != null)
                {
                    rival = rivPerson;
                    wo.SetAction(ActionEnum.Train);
                    if (rivPerson.rival != this||rival.wo.GetAction()!=ActionEnum.Train)
                    {
                        rivPerson.SetRival(wo);
                    }

                }
            }
        }

        public bool CanAct(WorldObject other)
        {
            return other != wo &&
                traits.CanAttack() &&
                other != null &&
                !other.IsDestroyed() &&
                wo.IsSameTeam(other.player) &&
                !wo.IsActionBlocking();
        }

        public void Act(WorldObject other)
        {
            SetRival(other);
        }
        public CommandEnum GetCommand()
        {
            return CommandEnum.Train;
        }

        public bool CanShowButton(WorldObject otherWO)
        {
            return otherWO != null && otherWO != wo && otherWO.IsSameTeam(wo.player) && otherWO.Owner.FindComponent<TrainBehavior>() != null;
        }

        public string GetCommandName(WorldObject otherWO)
        {
            return string.Format("Train {0}",otherWO.GetWoName());
        }
    }
}
