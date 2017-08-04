#region Using Statements
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace Codigo
{
    public class Game : WaveEngine.Framework.Game
    {
        public override void Initialize(IApplication application)
        {
            base.Initialize(application);
            ScreenContext screenContext = new ScreenContext(new PlayableScene());
           // ScreenContext screenContext = new ScreenContext(new MyScene());
            //ScreenContext screenContext = new ScreenContext(new TestScene(0));	
            WaveServices.ScreenContextManager.To(screenContext);
        }
    }
}
