using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.TiledMap;

namespace Codigo.Behaviors
{
   public class MovementBehaviorAlt:Behavior
    {
        [RequiredComponent]
        public WorldObject wo { get; private set; }


        public List<LayerTile> path { get; private set; }
        public LayerTile nextTile { get; private set; }
        private LayerTile currentTile;
        private Boolean erase;
        private Boolean moving;
        private Boolean changedTile;
        protected override void Initialize()
        {
            base.Initialize();
    
            path = new List<LayerTile>();
        }
        
        protected override void Update(TimeSpan gameTime)
        {
            if (nextTile != null)
            {
                Map map = Map.map;
                LayerTile tile=map.GetTileByWorldPosition(wo.transform.Position);
                Trace.WriteLine(tile);
                if (tile == nextTile&&!changedTile)
                {
                    map.SetTileOccupied(currentTile.X, currentTile.Y, false);
                    map.SetMobile(currentTile.X, currentTile.Y, null);
                    map.SetMobile(nextTile.X, nextTile.Y, wo);
                    map.SetTileOccupied(nextTile.X, nextTile.Y, true);
                    changedTile = true;
                }
                if (wo.transform.Position == nextTile.LocalPosition)
                {
                
                    if (erase)
                    {
                        nextTile = null;
                        currentTile = null;
                        path.Clear();
                        erase = false;
                        moving = false;
                    }
                    else
                    {
                        int index = path.IndexOf(nextTile);
                        if (!(index >= path.Count - 1))
                        {
                            LayerTile aux = path.ElementAt(index + 1);

                            // Aquí se comprueba si la casilla se encuentra ocupada en estos momentos
                            WorldObject otherWO = map.GetWorldObject(aux.X, aux.Y);
                            if (!map.IsTileOccupied(aux.X, aux.Y) && (otherWO == null || otherWO.IsTraversable(wo)))
                            {
                                
                                currentTile = nextTile;
                                nextTile = aux;
                                Move();
                            }
                        }
                        else
                        {
                            nextTile = null;
                            currentTile = null;
                            path.Clear();
                            moving = false;
                        }
                    }
                }
            }
        }
        //Método para mover al personaje
        private void Move()
        {
            changedTile = false;
            moving = true;
            Map.map.SetTileOccupied(nextTile.X, nextTile.Y, true);

            var animation = new WaveEngine.Components.GameActions.MoveTo2DGameAction(Owner, new Vector2(nextTile.LocalPosition.X, nextTile.LocalPosition.Y), TimeSpan.FromSeconds(3));
            animation.Run();
        }
        public void SetPath(List<LayerTile> list)
        {
            if (path.Count == 0 &&list.Count>1)
            {
                LayerTile aux = list.ElementAt(1);
                if (!Map.map.IsTileOccupied(aux.X, aux.Y))
                {
                    path.AddRange(list);
                    nextTile = aux;
                    currentTile = list.ElementAt(0);
                    Move();
                }

            }
            else Clear();
        }
        public void Clear()
        {
            erase = true;
        }
        public Boolean Moving()
        {
            return moving;
        }

    }
}
