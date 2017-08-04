using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;

namespace Codigo.Components
{
    public class PartialFog : Component
    {
        /**
         * <summary>
         * The original entity this component replaces in the partly visible tiles. 
         * This is actually the last WO that was seen in that tile
         * </summary>
         */
        public WorldObject originalEntity;

        [RequiredComponent]
        /**
         * <summary>
         * The sprite to be shown that represents the original entity
         * </summary>
         */
        public Sprite sprite;

    }
}
