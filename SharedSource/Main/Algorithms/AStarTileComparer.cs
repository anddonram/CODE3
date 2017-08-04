using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.TiledMap;

namespace Codigo.Algorithms
{
    public class AStarTileComparer : IComparer<LayerTile>
    {
        private Dictionary<LayerTile, float> fScore;

        public AStarTileComparer(Dictionary<LayerTile, float> fScore)
        {
            this.fScore = fScore;
        }

        public int Compare(LayerTile x, LayerTile y)
        {
            int comp = 0;
            if (x != y)
            {
                float fx = fScore.ContainsKey(x) ? fScore[x] : float.PositiveInfinity;
                float fy = fScore.ContainsKey(y) ? fScore[y] : float.PositiveInfinity;
                comp = fx.CompareTo(fy);
                if (comp == 0)
                {
                    comp = x.X.CompareTo(y.X);
                    if (comp == 0)
                    {
                        comp = x.Y.CompareTo(y.Y);
                    }
                }
            }
            return comp;
        }

    }
}
