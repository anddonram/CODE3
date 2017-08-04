using Codigo.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.TiledMap;

namespace Codigo
{
    /**
    * <summary>
    * This behavior creates a person and moves it.
    * Used to check the movement gameactions
    * </summary>
    * 
    */
    public   class MovementBehaviorTest : SceneBehavior
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
                List<LayerTile> tiles = new List<LayerTile>();
                tiles.Add(Map.map.GetTileByMapCoordinates(3, 3));
                tiles.Add(Map.map.GetTileByMapCoordinates(2, 3));
                tiles.Add(Map.map.GetTileByMapCoordinates(2, 2));
                tiles.Add(Map.map.GetTileByMapCoordinates(2, 1));
                p.FindComponent<MovementBehavior>().SetPath(tiles);

                p = ui.CreateToTile("Person", 1, 1);
                tiles = new List<LayerTile>();
                tiles.Add(Map.map.GetTileByMapCoordinates(1, 1));
                tiles.Add(Map.map.GetTileByMapCoordinates(1, 2));
                tiles.Add(Map.map.GetTileByMapCoordinates(2, 2));
                tiles.Add(Map.map.GetTileByMapCoordinates(2, 1));
                tiles.Add(Map.map.GetTileByMapCoordinates(2, 0));
                p.FindComponent<MovementBehavior>().SetPath(tiles);

            }
        }
       

    }
}
