using Codigo.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.TiledMap;

namespace Codigo
{
    /**
  * <summary>
  * This behavior shows the fog of war working
  * </summary>
  * 
  */
  [Obsolete]
    public class FogToggleBehavior : SceneBehavior
    {
        bool fogOn = false;
        bool previousState = false;
        

        protected override void ResolveDependencies()
        {
           
        }
        
        
        protected override void Update(TimeSpan gameTime)
        {
      
            bool currentState = WaveServices.Input.KeyboardState.P == WaveEngine.Common.Input.ButtonState.Pressed;

            if (FogOfWar.fog != null && currentState && !previousState)
            {
                FogOfWar.fog.ToggleFog(fogOn);
                fogOn = !fogOn;
            }
            previousState = currentState;
        }
    }

}
