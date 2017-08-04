using Codigo.Behaviors;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.TiledMap;

namespace Codigo
{
    /// <summary>
    /// Mouse drawable class
    /// </summary>
    [DataContract]
    public class PathDrawable : Drawable2D
    {
        private List<LayerTile> path=new List<LayerTile>();
        private Player player;
        private Layer drawLayer;

        public void SetPlayer(Player player,List<LayerTile> path)
        {
            this.player = player;
            this.path = path;
        }
        /// <summary>
        /// Resolve dependencies method
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            this.drawLayer = this.RenderManager.FindLayer(DefaultLayers.Alpha);
        }

        /// <summary>
        /// Draw method
        /// </summary>
        /// <param name="gameTime">game time</param>
        public override void Draw(TimeSpan gameTime)
        {
            if (player != null && player.active)
            {
                List<LayerTile> drawPath = null;
                if (path.Count > 1)
                {
                    drawPath = path;
                }
                else if (player.active && player.selectedWO != null && !player.selectedWO.IsDestroyed() && player.selectedWO.player == player && player.selectedWO.IsMobile())
                {
                    drawPath = player.selectedWO.Owner.FindComponent<MovementBehavior>().path;
                }
                if (drawPath != null && drawPath.Count > 1)
                {
                    LayerTile start = drawPath[0];
                    LayerTile end = drawPath[1];
                    Vector2 first = start.LocalPosition + WorldObjectData.center;
                    if (player.selectedWO != null)
                    {
                        first = player.selectedWO.GetCenteredPosition();
                    }
                    drawLayer.LineBatch2D.DrawLine(first, end.LocalPosition + WorldObjectData.center, player.playerColor, 0);
                    start = end;
                    for (int i = 2; i < drawPath.Count; i++)
                    {
                        end = drawPath[i];
                        drawLayer.LineBatch2D.DrawLine(start.LocalPosition + WorldObjectData.center, end.LocalPosition + WorldObjectData.center, player.playerColor, 0);
                        start = end;
                    }
                }
                if (player.selectedWO != null)
                {//we'll outline the selected world object tile
                    WorldObject wo = player.selectedWO;
                    LayerTile tile = Map.map.GetTileByMapCoordinates(wo.GetX(), wo.GetY());
                    if (FogOfWar.fog.IsVisible(wo.GetX(), wo.GetY()))
                    {
                        drawLayer.LineBatch2D.DrawLine(tile.LocalPosition, tile.LocalPosition + Vector2.UnitX *  WorldObjectData.tileSize.X, player.playerColor, 0);
                        drawLayer.LineBatch2D.DrawLine(tile.LocalPosition, tile.LocalPosition + Vector2.UnitY * WorldObjectData.tileSize.Y, player.playerColor, 0);
                        drawLayer.LineBatch2D.DrawLine(tile.LocalPosition + WorldObjectData.tileSize, tile.LocalPosition + Vector2.UnitX * WorldObjectData.tileSize.X, player.playerColor, 0);
                        drawLayer.LineBatch2D.DrawLine(tile.LocalPosition + WorldObjectData.tileSize, tile.LocalPosition + Vector2.UnitY * WorldObjectData.tileSize.Y, player.playerColor, 0);
                    }
                }
                if (player.currentTile != null)
                {//we'll outline the mouse position tile
                    LayerTile tile = player.currentTile;
                    drawLayer.LineBatch2D.DrawLine(tile.LocalPosition, tile.LocalPosition + Vector2.UnitX * WorldObjectData.tileSize.X, player.playerColor, 0);
                    drawLayer.LineBatch2D.DrawLine(tile.LocalPosition, tile.LocalPosition + Vector2.UnitY * WorldObjectData.tileSize.Y, player.playerColor, 0);
                    drawLayer.LineBatch2D.DrawLine(tile.LocalPosition + WorldObjectData.tileSize, tile.LocalPosition + Vector2.UnitX * WorldObjectData.tileSize.X, player.playerColor, 0);
                    drawLayer.LineBatch2D.DrawLine(tile.LocalPosition + WorldObjectData.tileSize, tile.LocalPosition + Vector2.UnitY * WorldObjectData.tileSize.Y, player.playerColor, 0);

                }
                if (player.lastActionTile != null)
                {
                    //we'll outline the last tile that was
                    LayerTile tile = player.lastActionTile;
                    drawLayer.LineBatch2D.DrawLine(tile.LocalPosition, tile.LocalPosition + Vector2.UnitX * WorldObjectData.tileSize.X, Color.White, 0);
                    drawLayer.LineBatch2D.DrawLine(tile.LocalPosition, tile.LocalPosition + Vector2.UnitY * WorldObjectData.tileSize.Y, Color.White, 0);
                    drawLayer.LineBatch2D.DrawLine(tile.LocalPosition + WorldObjectData.tileSize, tile.LocalPosition + Vector2.UnitX * WorldObjectData.tileSize.X, Color.White, 0);
                    drawLayer.LineBatch2D.DrawLine(tile.LocalPosition + WorldObjectData.tileSize, tile.LocalPosition + Vector2.UnitY * WorldObjectData.tileSize.Y, Color.White, 0);

                }

            }
        }
    

        /// <summary>
        /// Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
        }
    }
}
