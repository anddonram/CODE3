using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace Codigo.Behaviors
{
    /**
     * <summary>
     * This class removes the tiles that were changed in the previous frame.
     * We need that lapse of time because otherwise, 
     * the d* algorithms for every mobile won't have time to recompute the edge costs for those tiles, resulting in a wrong behavior.
     * </summary>
     */
    public class FogClearBehavior : SceneBehavior
    {
        protected override void ResolveDependencies()
        {
            
        }
        /**
         * <summary>
         * the number of tiles to remove from the updated tiles at the end of this frame
         * </summary>
         */
        private int tilesToClear = 0;
        protected override void Update(TimeSpan gameTime)
        { 
            FogOfWar.fog.updatedTiles.RemoveRange(0, tilesToClear);
            tilesToClear = FogOfWar.fog.tilesAdded;
            FogOfWar.fog.tilesAdded = 0;
        }
    }
}
