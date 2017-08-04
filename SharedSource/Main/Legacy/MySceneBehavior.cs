using Codigo.Behaviors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.TiledMap;
using WaveEngine.Components.UI;
using System.Linq;
using WaveEngine.Common.Graphics;

namespace Codigo
{
    /**
     *General behavior in the game should be place here 
     */
    [Obsolete]
    public class MySceneBehavior : SceneBehavior
    {
        private Transform2D camTransform;
        private Camera2D cam;
        private Map map;

    
        protected override void ResolveDependencies()
        {
        }
        
        public void SetMap(Entity mapEntity)
        {
            map = Map.map;
            cam = Scene.RenderManager.ActiveCamera2D;
            camTransform = cam.Transform;

            
            
            keysBehavior = camTransform.Owner.FindComponent<KeysBehavior>();

        }



        /**
         * <summary>
         * This updates the state of the mouse and keyboard, to detect presses and releases.
         * </summary>
         */
        KeysBehavior keysBehavior;
        protected override void Update(TimeSpan gameTime)
        {
    
         
            KeyboardState keyboard = WaveServices.Input.KeyboardState;
            MouseState currentMouse = WaveServices.Input.MouseState;
          
                if (keysBehavior.IsCommandExecuted(CommandEnum.ToggleFog))
                {
                    Trace.WriteLine(string.Format("Keyboard P state = {0,8} Keyboard O state = {1,8}", keyboard.P, keyboard.O));
                }
          

                if (keysBehavior.IsCommandExecuted(CommandEnum.StartPath))
                {
                    ShowMapInfo();
                }
               
            }


        /**
        * creates a thing in the tile where the camera points.
        */
        [Obsolete]
        private void CreateThing()
        {
            LayerTile tile = map.GetTileByWorldPosition(camTransform.Position);
            if (tile!=null&&!map.IsTileOccupied(tile.X, tile.Y))
            {
                WorldObject wo = new WorldObject();
                Entity entity = new Entity()
                    .AddComponent(new Transform2D
                    {
                        Position = tile.LocalPosition
                    })
                    .AddComponent(new Sprite(WaveContent.Assets.ball_png))
                    .AddComponent(new SpriteRenderer())
                    .AddComponent(wo);
                map.SetWorldObject(tile.X, tile.Y,wo);
                Scene.EntityManager.Add(entity);
            }
        }

        /**
         * <summary>
         * show auxiliar info about the map, including the tile in the center of the camera
         * </summary>
         */
        private void ShowMapInfo()
        {
            //get info about map
            Trace.WriteLine(string.Format("Map size:height={0}, width={1}",map.tiledMap.Height, map.tiledMap.Width));

            //A tileset is the set of tiles used for painting de tmx
            Tileset set = map.tiledMap.GetTilesetByGid(17);
            if (set != null)
            {
                Trace.WriteLine(string.Format("tileset id: {0}",set.FirstGid));
            }

            //tile in the camera center
            LayerTile tile = map.GetTileByWorldPosition(camTransform.Position);
            if (tile != null)
            {
                Trace.WriteLine(tile.ToString());
                Trace.WriteLine(string.Format("Tile coordinate:x={0}, y={1}", tile.X, tile.Y));
                Trace.WriteLine(string.Format("tile type: {0}", tile.Tileset.FirstGid));
            }

            // shows the point from the mouse in world coordinates
            Trace.WriteLine(string.Format("mouse on: {0}", GetMousePosition()));
        }

        private Vector2 GetMousePosition() {
            Vector2 position = Vector2.Zero;
            Vector2 mouse = WaveServices.Input.MouseState.Position;
            cam.Unproject(ref mouse, out position);
            return position;
        }
     
    }
}
