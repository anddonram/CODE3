using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;

namespace Codigo.Components
{
   public class CottageFogAdjacents:FogAdjacents
    {

        [RequiredComponent]
        private Cottage building;

        /**
         * <summary>
         * The occupation in the last frame so visibility can be changed
         * </summary>
         */
        private bool lastOccupation=false;
        /**
          * <summary>
          * Tiles visible when cottage is occupied
          * </summary>
          */
        private List<Point> fullLight;
        /**
           * <summary>
           * Tiles visible when building or empty
           * </summary>
           */
        private List<Point> emptyLight;


        private List<Point> lastAdjacents=null;

        protected override void Initialize()
        {
            emptyLight = new List<Point>();
            emptyLight.Add(new Point(0, 0));

            fullLight = new List<Point>(emptyLight);
            fullLight.Add(new Point(0, 1));
            fullLight.Add(new Point(1, 0));
            fullLight.Add(new Point(1, 1));
            fullLight.Add(new Point(0, -1));
            fullLight.Add(new Point(-1, 0));
            fullLight.Add(new Point(-1, -1));
            fullLight.Add(new Point(1, -1));
            fullLight.Add(new Point(-1, 1));

            //first, the light is at buildingLight
            adjacents = emptyLight;
        }
  
        public override List<Point> GetAdjacents()
        {
            return adjacents;
        }
        public override bool VisibilityChanged()
        {

            bool currentOccupation = (building.GetPeople().Count > 0);

            bool res = false;
            if (currentOccupation!=lastOccupation)
            {
                res = true;
                lastOccupation =currentOccupation;
                lastAdjacents = adjacents;
                if (currentOccupation)
                {
                    adjacents = fullLight;
                }else
                {
                    adjacents = emptyLight;
                }
                

            }
            return res;
        }
        public override List<Point> LastAdjacents() {
            return lastAdjacents;
        }
    }
}
