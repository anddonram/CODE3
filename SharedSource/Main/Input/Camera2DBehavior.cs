using System;
using System.Diagnostics;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.TiledMap;

namespace Codigo
{
    public class Camera2DBehavior : Behavior
    {
        private float speed = 64,zoomSpeed=0.02f;
        private float tileHeight, tileWidth;
        /**
         * <summary>
         * The minimum and maximum zoom scale that can be achieved
         * </summary>
         */
        private Vector2 minZoom=Vector2.One*0.2f, maxZoom=Vector2.One*0.9f;
        /**
         * <summary>
         * the bounds of the map, inside a rectangle
         * </summary>
         */
        private Vector2 topLeftCorner, bottomRightCorner;

        [RequiredComponent]
        private Transform2D transform;

        [RequiredComponent]
        private KeysBehavior keys;

        protected override void Update(TimeSpan gameTime)
        {
            HandleKeyboard((float)gameTime.TotalSeconds);
            HandleMouse((float)gameTime.TotalSeconds);
        }

        /// <summary>
        /// Set the camera at its initial position, depending on the map size
        /// </summary>
        public void SetDefaultValues()
        {
            TiledMap map = Map.map.tiledMap;
            topLeftCorner = map.Owner.FindComponent<Transform2D>().Position;
            tileWidth = map.TileWidth;
            tileHeight = map.TileHeight;
            bottomRightCorner = topLeftCorner + new Vector2(Map.map.width*tileWidth,Map.map.height*tileHeight);
            transform.Position = (topLeftCorner + bottomRightCorner) / 2;


            Trace.WriteLine(topLeftCorner);
            Trace.WriteLine(transform.Position);
            Trace.WriteLine(bottomRightCorner);            
        }


        /// <summary>
        /// Move the camera using the Keyboard
        /// </summary>
        /// <param name="amount">The amount of time, frame-independent thing</param>
        private void HandleKeyboard(float amount)
        {
            KeyboardState keyboardState = WaveServices.Input.KeyboardState;
            
            if (keys.IsCommandExecuted(CommandEnum.CameraUp) && !(transform.Y < topLeftCorner.Y+tileHeight))
            {
                transform.Y-=speed*amount;
            }
            else if (keys.IsCommandExecuted(CommandEnum.CameraDown) && !(transform.Y > bottomRightCorner.Y-tileHeight))
            {
                transform.Y += speed* amount;
            }

            if (keys.IsCommandExecuted(CommandEnum.CameraLeft) && !(transform.X<topLeftCorner.X+tileWidth))
            {
                transform.X -= speed* amount;
            }
            else if (keys.IsCommandExecuted(CommandEnum.CameraRight) && !(transform.X > bottomRightCorner.X-tileWidth))
            {
                transform.X += speed* amount;
            }
            else if (keys.IsCommandExecuted(CommandEnum.CameraCenter))
            {
                Player selected = UIBehavior.ui.activePlayer;
                if (selected != null && selected.selectedWO != null)
                {
                    transform.Position = selected.selectedWO.transform.Position;
                }
            }
        }

        /// <summary>
        ///Zoom in and out using the mouse wheel
        /// </summary>
        /// <param name="amount">The amount of time, frame-independent thing</param>
        private void HandleMouse(float amount)
        {
            MouseState mouse = WaveServices.Input.MouseState;
            if (mouse.IsConnected)
            {
                float zoomFactor = amount * keys.Zoom() * zoomSpeed;
                Vector2 newScale = transform.Scale + zoomFactor * Vector2.One;

                newScale = Vector2.Clamp(newScale, minZoom, maxZoom);
                if (newScale != transform.Scale)
                {
                    transform.Scale = newScale;
                }
            }
        }

    }
}