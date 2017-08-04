using Codigo.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.TiledMap;

namespace Codigo.Behaviors
{
   public class MovementBehavior:Behavior
    {
        [RequiredComponent]
        private WorldObject wo;
        

        public List<LayerTile> path { get; private set; }
        public LayerTile nextTile { get; private set; }
        private LayerTile currentTile;
        private Boolean erase;
        /**
         * <summary>
         * whether we have changed to the next tile but we have not reached the next position
         * </summary>
         */
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
                if (wo.GetAction() == ActionEnum.Idle)
                {
                    wo.SetAction(ActionEnum.Move);
                    Clear();
                }
                Map map = Map.map;
                LayerTile tile = map.GetTileByWorldPosition(wo.GetCenteredPosition());
                if(tile==currentTile && !changedTile)
                {
                    if(map.GetWorldObject(nextTile.X, nextTile.Y)!=null && !map.GetWorldObject(nextTile.X, nextTile.Y).IsTraversable(wo))
                    {
                        Trace.WriteLine("AGUAAAAAAAAAAAA");
                        LayerTile aux = nextTile;
                        nextTile = currentTile;
                        changedTile = true;
                        currentTile = aux;
                        path.Clear();
                        path.Add(currentTile);
                        path.Add(nextTile);
                        Map.map.SetTileOccupied(currentTile.X, currentTile.Y, true);
                        float distanceTile = Vector2.Distance(currentTile.LocalPosition, nextTile.LocalPosition);
                        float distancePerson = Vector2.Distance(nextTile.LocalPosition, wo.transform.Position);
                        if (wo.animation != null)
                        {
                            wo.animation.Cancel();
                        }                        
                        wo.animation = new WaveEngine.Components.GameActions.MoveTo2DGameAction(Owner, nextTile.LocalPosition, TimeSpan.FromSeconds(wo.genericSpeed*distancePerson/distanceTile));
                        wo.animation.Run();
                        wo.SetAction(ActionEnum.Move);
                    }
                }
                if (tile == nextTile && !changedTile)
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
                        Reset();
                        if (recalculating != null && (wo.pendingActions.Count == 0|| recalculating.Count > 2))
                        {
                            SetPath(recalculating);
                            recalculating = null;
                        }
                        else
                        {
                            recalculating = null;
                            wo.ExecuteAction();
                        }
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
                            else
                            {
                                Clear();
                            }
                        }
                        else
                        {
                            //Path finished!!
                            Reset();
                            wo.ExecuteAction();
                        }
                    }
                }
             
            }
        }
        /**
         * <summary>
         * Called when path finished or aborted
         * </summary>
         */
        private void Reset()
        {
            nextTile = null;
            currentTile = null;
            path.Clear();
            erase = false;
            wo.SetAction(ActionEnum.Idle);
        }

        /**
        * <summary>
        * Makes this person move to the next tile
        * </summary>
        */
        private void Move()
        {
            changedTile = false;

            if (wo.GetAction() != ActionEnum.Exit)
            {
                wo.SetAction(ActionEnum.Move);
            }
            
            Map.map.SetTileOccupied(nextTile.X, nextTile.Y, true);

            wo.animation=new WaveEngine.Components.GameActions.MoveTo2DGameAction(Owner, nextTile.LocalPosition, TimeSpan.FromSeconds(wo.genericSpeed));
            wo.animation.Run();
        }
        /**
          * <summary>
         * Makes this person move to the next tile
         * </summary>
         */
        public void SetPath(List<LayerTile> list)
        {
            if (path.Count == 0 &&list.Count>1)
            {
                LayerTile aux2 = list.ElementAt(0);
                LayerTile aux = list.ElementAt(1);
                WorldObject otherWO = Map.map.GetWorldObject(aux.X, aux.Y);
                Trace.WriteLine(wo.GetX() + ", " + wo.GetY());
                if (!Map.map.IsTileOccupied(aux.X, aux.Y) && (otherWO == null || otherWO.IsTraversable(wo)) && aux2.X == wo.GetX() && aux2.Y == wo.GetY())
                {
                    path.AddRange(list);
                    nextTile = list.ElementAt(1);
                    currentTile = list.ElementAt(0);
                    Move();
                }
            }
            else Clear();
        }
        /**
         * <summary>
         * Forces the behavior to clear the current path
         * </summary>
         */
        public void Clear()
        {
            erase = true;
            recalculating = null;
        }
        /**
         * <summary>
         * holds the new path forced by D*
         * </summary>
         */
        private List<LayerTile> recalculating=null;
        /**
         * <summary>
         * The D* forces a new path, even if it is moving
         * </summary>
         */
        public void ForcePath(List<LayerTile> dPath)
        {
            if (!erase&&!wo.IsDestroyed() && wo.GetAction() == ActionEnum.Move&&dPath.Count>1)
            {
                erase = true;
                recalculating = dPath;
            }
        }
        /**
         * <summary>
         * this is true if we want to stop the movement and not for a recalculating
         * </summary>
         */
        public bool FullStop()
        {
            return erase && recalculating == null;
        }
        public void MoveBack(LayerTile oldTile, LayerTile newTile)
        {
            nextTile = oldTile;
            currentTile = newTile;
            path.Clear();
            path.Add(newTile);
            path.Add(oldTile);
            changedTile = true;
            if (wo.animation != null)
            {
                wo.animation.Cancel();
            }
            Map.map.SetMobile(nextTile.X, nextTile.Y, wo);
            Map.map.SetTileOccupied(nextTile.X, nextTile.Y, true);
            float distanceTile = Vector2.Distance(currentTile.LocalPosition, nextTile.LocalPosition);
            float distancePerson = Vector2.Distance(nextTile.LocalPosition, wo.transform.Position);
            wo.animation = new WaveEngine.Components.GameActions.MoveTo2DGameAction(Owner, oldTile.LocalPosition, TimeSpan.FromSeconds(wo.genericSpeed * distancePerson / distanceTile));
            wo.animation.Run();
            wo.SetAction(ActionEnum.Move);
        }
    }
}
