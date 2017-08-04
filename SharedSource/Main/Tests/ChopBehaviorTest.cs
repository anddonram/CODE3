using Codigo.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace Codigo
{
    /**
    * <summary>
    * This behavior creates a person and moves it.
    * Used to check the movement gameactions
    * </summary>
    * 
    */
    public   class ChopBehaviorTest : SceneBehavior
    {
        bool started = false;

        protected override void ResolveDependencies()
        {
       
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (!started) {
                started = true;
                UIBehavior ui=Scene.EntityManager.Find("camera2D").FindComponent<UIBehavior>();
                Entity p=ui.CreateToTile("Person", 3, 3);
                Entity t=ui.CreateToTile("Tree", 4, 3);
                p.FindComponent<ChopBehavior>().Chop(t.FindComponent<WorldObject>());

            }
        }
       

    }
}
