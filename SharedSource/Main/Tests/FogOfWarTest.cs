using Codigo.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.TiledMap;

namespace Codigo.Tests
{
    /**
  * <summary>
  * This behavior shows the fog of war working
  * </summary>
  * 
  */
    public class FogOfWarTest : SceneBehavior
    {
        bool started = false;

        
        protected override void ResolveDependencies()
        {

        }
        
        protected override void Update(TimeSpan gameTime)
        {
            if (!started)
            {
                started = true;

                UIBehavior ui = UIBehavior.ui;


                Entity p = ui.CreateToTile("Person", 3, 3);
                
 
                List<LayerTile> tiles = new List<LayerTile>();
                tiles.Add(Map.map.GetTileByMapCoordinates(3, 3));
                tiles.Add(Map.map.GetTileByMapCoordinates(2, 3));
                tiles.Add(Map.map.GetTileByMapCoordinates(2, 2));
                tiles.Add(Map.map.GetTileByMapCoordinates(2, 1));
                tiles.Add(Map.map.GetTileByMapCoordinates(3, 1));
                p.FindComponent<MovementBehavior>().SetPath(tiles);
                p = ui.CreateToTile("FakeTree", 2, 2);
               
                ui.playerButtons[1].IsChecked = true;

                p = ui.CreateToTile("FakeTree", 1, 2);
                p = ui.CreateToTile("Person", 1, 1);
                
                tiles = new List<LayerTile>();
                tiles.Add(Map.map.GetTileByMapCoordinates(1, 1));
                tiles.Add(Map.map.GetTileByMapCoordinates(1, 2));
                tiles.Add(Map.map.GetTileByMapCoordinates(2, 2));
                tiles.Add(Map.map.GetTileByMapCoordinates(2, 1));
                tiles.Add(Map.map.GetTileByMapCoordinates(2, 0));
                tiles.Add(Map.map.GetTileByMapCoordinates(1, 0));
                tiles.Add(Map.map.GetTileByMapCoordinates(0, 0));
                tiles.Add(Map.map.GetTileByMapCoordinates(0, 1));
                p.FindComponent<MovementBehavior>().SetPath(tiles);
             
            }
            
        }
    }

}
