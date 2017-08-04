using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.TiledMap;

namespace Codigo.Behaviors
{
    [Obsolete]
    public class MovementBehaviorLegacy : Behavior
    {
        [RequiredComponent]
        public WorldObject wo { get; private set; }


        public List<LayerTile> path { get; private set; }
        public LayerTile nextTile { get; private set; }
        private LayerTile currentTile;
        private Boolean erase;
        private Boolean moving;

        protected override void Initialize()
        {
            base.Initialize();

            path = new List<LayerTile>();
        }
        protected override void Update(TimeSpan gameTime)
        {
            if (nextTile != null)
            {
                if (wo.transform.Position == nextTile.LocalPosition)
                {
                    Map map = EntityManager.Find("map").FindComponent<Map>();
                    map.SetTileOccupied(currentTile.X, currentTile.Y, false);
                    map.SetMobile(currentTile.X, currentTile.Y, null);
                    map.SetMobile(nextTile.X, nextTile.Y, wo);
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
                            /*
                             * Aquí se comprueba si la casilla se encuentra ocupada en estos momentos*/
                            if (!map.IsTileOccupied(aux.X, aux.Y))
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
        public void Move()
        {
            moving = true;
            Map.map.SetTileOccupied(nextTile.X, nextTile.Y, true);

            var animation = new WaveEngine.Components.GameActions.MoveTo2DGameAction(Owner, new Vector2(nextTile.LocalPosition.X, nextTile.LocalPosition.Y), TimeSpan.FromSeconds(3));
            animation.Run();
        }
        public void SetPath(List<LayerTile> list)
        {
            if (path.Count == 0 && list.Count > 1)
            {
                LayerTile aux = list.ElementAt(1);
                if (!Map.map.IsTileOccupied(aux.X, aux.Y))
                {
                    path.AddRange(list);
                    nextTile = list.ElementAt(1);
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
