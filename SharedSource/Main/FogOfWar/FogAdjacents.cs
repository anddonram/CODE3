using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;

namespace Codigo.Components
{
    /**
     * <summary>
     * This class handles the tiles that will be revealed. If an entity does not hold an instance of this or its subclasses, he won't reveal anything
     * </summary>
     */
    public class FogAdjacents:Component
    {
        protected List<Point> adjacents;
        protected override void Initialize()
        {
            base.Initialize();
            adjacents = new List<Point>();
            //The visible tiles around it
            adjacents.Add(new Point(0, 0));
            adjacents.Add(new Point(0, 1));
            adjacents.Add(new Point(1, 0));
            adjacents.Add(new Point(1, 1));
            adjacents.Add(new Point(0, -1));
            adjacents.Add(new Point(-1, 0));
            adjacents.Add(new Point(-1, -1));
            adjacents.Add(new Point(1, -1));
            adjacents.Add(new Point(-1, 1));
        }
  
        public virtual List<Point> GetAdjacents() {
            return adjacents;
        }
        public virtual bool VisibilityChanged()
        {
            return false;
        }
        public virtual List<Point> LastAdjacents()
        {
            return adjacents;
        }
    }
}
