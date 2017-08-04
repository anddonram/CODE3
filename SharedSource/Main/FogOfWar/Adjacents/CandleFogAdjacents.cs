using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;

namespace Codigo.Components
{
   public class CandleFogAdjacents:FogAdjacents
    {
        /**
         * <summary>
         * If health is greater than this percentage, candle is at full light
         * </summary>
         */
        private float fullLightPercentage = 0.5f;
        /**
         * <summary>
         * If health is greater than this percentage, candle is at half light
         * </summary>
         */
        private float halfLightPercentage = 0.2f;
 
        [RequiredComponent(false)]
        private Building building;

        private bool underConstruction=true;

        private List<Point> lastAdjacents=null;

        /**
         * <summary>
         * Tiles visible when at full light
         * </summary>
         */
        private List<Point> fullLight;
        /**
          * <summary>
          * Tiles visible when at less light
          * </summary>
          */
        private List<Point> halfLight;
        /**
          * <summary>
          * Tiles visible when almost destroyed
          * </summary>
          */
        private List<Point> minLight;
        /**
           * <summary>
           * Tiles visible when building
           * </summary>
           */
        private List<Point> buildingLight;
        /**
         * <summary>
         * last health to compare if visibility must change
         * </summary>
         */
        private float lastHealth=0f;

        protected override void Initialize()
        {
            buildingLight = new List<Point>();
            buildingLight.Add(new Point(0, 0));
            minLight = new List<Point>(buildingLight);
            minLight.Add(new Point(0, 1));
            minLight.Add(new Point(1, 0));
            minLight.Add(new Point(-1, 0));
            minLight.Add(new Point(0, -1));

            halfLight = new List<Point>(minLight);
            halfLight.Add(new Point(1, 1));
            halfLight.Add(new Point(-1, -1));
            halfLight.Add(new Point(1, -1));
            halfLight.Add(new Point(-1, 1));

            fullLight = new List<Point>(halfLight);
            fullLight.Add(new Point(2, 0));
            fullLight.Add(new Point(-2, 0));
            fullLight.Add(new Point(0, 2));
            fullLight.Add(new Point(0, -2));

            //first, the light is at buildingLight
            adjacents = buildingLight;
        }
  
        public override List<Point> GetAdjacents()
        {
            return adjacents;
        }
        public override bool VisibilityChanged()
        {
            bool res = underConstruction && !building.UnderConstruction();
            if (res)
            {
                underConstruction = false;
                lastAdjacents = adjacents;
                adjacents = fullLight;
                lastHealth = building.wo.GetHealth();

            }
            else  if (!underConstruction)
            {
                if (building.wo.GetHealth()!=lastHealth)
                {
                    res = true;
                    float maxHealth = building.wo.GetMaxHealth();
                    lastHealth = building.wo.GetHealth();
                    lastAdjacents = adjacents;
                    if (lastHealth >= maxHealth * fullLightPercentage)
                    {
                        adjacents = fullLight;
                    }else if(lastHealth >= maxHealth * halfLightPercentage)
                    {
                        adjacents = halfLight;
                    }else
                    {
                        adjacents = minLight;
                    }

                }
            }
            return res;
        }
        public override List<Point> LastAdjacents() {
            return lastAdjacents;
        }
    }
}
