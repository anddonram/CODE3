using System;
using System.Collections.Generic;
using WaveEngine.Framework;
using System.Text;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Common.Graphics;

namespace Codigo.Components
{
    public class Building : Component
    {
        /**
         * <summary>
         * Cannot be private because some external components need it
         * </summary>
         */
        [RequiredComponent]
        public WorldObject wo;


        protected override void Initialize()
        {
            wo.SetAction(ActionEnum.Build);
            Owner.FindComponent<Sprite>().TintColor = new Color(1,1,1,0.8f);
        }

        public Boolean Build(float quantity)
        {
            if (UnderConstruction())
            {
                wo.Heal(quantity);
                if (wo.GetHealth() == wo.GetMaxHealth())
                {
                    FinishBuilding();
                }
            }
            return !UnderConstruction();
        }
        public bool UnderConstruction()
        {
            return wo.GetAction() == ActionEnum.Build;
        }
        /**
         * <summary>
         * Do special things when finished
         * </summary>
         */
        protected virtual void FinishBuilding()
        {
            wo.SetAction(ActionEnum.Idle);

            Owner.FindComponent<Sprite>().TintColor = Color.White;
        }
    }
}
