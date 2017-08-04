
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.TiledMap;

namespace Codigo.Algorithms
{
    public class DStarTileComparer : IComparer<LayerTile>
    {
        private Dictionary<LayerTile, Vector2> fScore;

        public DStarTileComparer(Dictionary<LayerTile, Vector2> fScore)
        {
            this.fScore = fScore;
        }

        public int Compare(LayerTile x, LayerTile y)
        {
            int comp = 0;
            if (x != y)
            {
                Vector2 fx = fScore.ContainsKey(x) ? fScore[x] : float.PositiveInfinity * Vector2.One;
                Vector2 fy = fScore.ContainsKey(y) ? fScore[y] : float.PositiveInfinity * Vector2.One;
                comp = fx.X.CompareTo(fy.X);
                if (comp == 0)
                {
                    comp = fx.Y.CompareTo(fy.Y);
                    if (comp == 0)
                    {
                        comp = x.X.CompareTo(y.X);
                        if (comp == 0)
                        {
                            comp = x.Y.CompareTo(y.Y);
                        }
                    }
                }
            }
            return comp;
        }

    }
}
