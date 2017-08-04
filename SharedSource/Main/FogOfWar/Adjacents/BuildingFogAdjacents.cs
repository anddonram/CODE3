using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;

namespace Codigo.Components
{
   public class BuildingFogAdjacents:FogAdjacents
    {

        [RequiredComponent(false)]
        private Building building;

        private bool underConstruction=true;

        private List<Point> lastAdjacents=null;

        protected override void Initialize()
        {
            adjacents = new List<Point>();
            adjacents.Add(new Point(0, 0));
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
                lastAdjacents = new List<Point>(adjacents);
                adjacents.Add(new Point(0, 1));
                adjacents.Add(new Point(1, 0));
                adjacents.Add(new Point(1, 1));
                adjacents.Add(new Point(0, -1));
                adjacents.Add(new Point(-1, 0));
                adjacents.Add(new Point(-1, -1));
                adjacents.Add(new Point(1, -1));
                adjacents.Add(new Point(-1, 1));
            }
            return res;
        }
        public override List<Point> LastAdjacents() {
            return lastAdjacents;
        }
    }
}
