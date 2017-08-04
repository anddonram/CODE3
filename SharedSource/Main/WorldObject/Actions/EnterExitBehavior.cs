using Codigo.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.TiledMap;
using WaveEngine.Framework.Services;

namespace Codigo.Behaviors
{
    class EnterExitBehavior:Behavior,ActionBehavior
    {
        [RequiredComponent]
        private WorldObject wo;

        public LayerTile nextTile { get; private set; }
        private LayerTile currentTile;
        /**
         * <summary>
         * The cottage we want to enter in
         * </summary>
         */
        private Cottage cottage;

        
        protected override void Update(TimeSpan gameTime)
        {
            if (wo.GetAction() == ActionEnum.Enter)
            {
                LayerTile tile = Map.map.GetTileByWorldPosition(wo.GetCenteredPosition());
                if (tile == nextTile)
                {
                    if (cottage != null && !cottage.IsDestroyed())
                    {
                        cottage.AddPerson(wo);
                        cottage = null;
                        if (Map.map.GetWorldObject(currentTile.X, currentTile.Y) == null || Map.map.GetWorldObject(currentTile.X, currentTile.Y).IsTraversable(wo))
                        {
                            //This is common execution path. however, if we came from a water tile because the bridge was destroyed
                            //this should not, and do not, run, effectively not setting the water tile to false, which is not right
                            Map.map.SetTileOccupied(currentTile.X, currentTile.Y, false);
                            Map.map.SetMobile(currentTile.X, currentTile.Y, null);
                        }

                        //nullify coordinates so when destroyed wont set anything free
                        wo.SetX(-1);
                        wo.SetY(-1);
                        nextTile = null;
                        currentTile = null;
                        cottage = null;
                        wo.SetAction(ActionEnum.Inside);
                    }else
                    {
                        ExitCottage(nextTile,currentTile);   
                    }
                }
            }
           else if (wo.GetAction() == ActionEnum.Exit)
            {
                Map map = Map.map;
                
                LayerTile tile = Map.map.GetTileByWorldPosition(wo.GetCenteredPosition());
                if (tile == nextTile)
                {
                    if (map.GetWorldObject(nextTile.X, nextTile.Y) != null && !map.GetWorldObject(nextTile.X, nextTile.Y).IsTraversable(wo))
                    { 
                        if (cottage != null&&!cottage.IsDestroyed())
                        {
                            //If we found water, return back
                            EnterCottage(cottage.wo);
                        }
                        else
                        {
                            //If we found water and the cottage is destroyed, back luck
                            wo.Destroy();
                        }

                    }
                    else
                    {
                        Map.map.SetMobile(nextTile.X, nextTile.Y, wo);
                        if (tile.LocalPosition == wo.transform.Position)
                        {
                            wo.Stop();
                            nextTile = null;
                            currentTile = null;
                            cottage = null;
                        }
                    }
                }
                
            }
        
        }
        public void ExitCottage(LayerTile cottage,LayerTile exit)
        {
            if (Map.map.GetWorldObject(cottage.X, cottage.Y) != null)
                this.cottage = Map.map.GetWorldObject(cottage.X, cottage.Y).Owner.FindComponent<Cottage>();
            nextTile = exit;
            currentTile = cottage;
            Map.map.SetTileOccupied(nextTile.X, nextTile.Y, true);
            if (wo.animation != null)
            {
                wo.animation.Cancel();
            }

            wo.animation= new WaveEngine.Components.GameActions.MoveTo2DGameAction(Owner, nextTile.LocalPosition, TimeSpan.FromSeconds(wo.genericSpeed));
            wo.animation.Run();
            wo.SetAction(ActionEnum.Exit);
        }


        public void EnterCottage(WorldObject  cot)
        {
            
                if (CanAct(cot))
                {
                    Cottage cottage = cot.Owner.FindComponent<Cottage>();
                    LayerTile l1 = Map.map.GetTileByWorldPosition(cottage.wo.GetCenteredPosition());
                    LayerTile l2 = Map.map.GetTileByWorldPosition(wo.GetCenteredPosition());

                    cottage.SetCount(cottage.GetCount() + 1);
                    nextTile = l1;
                    currentTile = l2;
                    this.cottage = cottage;

                    Trace.WriteLine(cottage.GetCount());
                    wo.SetAction(ActionEnum.Enter);
                    if (wo.animation != null)
                    {
                        wo.animation.Cancel();
                    }
                    wo.animation = new WaveEngine.Components.GameActions.MoveTo2DGameAction(Owner, l1.LocalPosition, TimeSpan.FromSeconds(wo.genericSpeed));
                    wo.animation.Run();
                }
            
        }

        public bool CanAct(WorldObject other)
        {
            bool res = other != null && !other.IsDestroyed();
            if (res) {
                Cottage cottage = other.Owner.FindComponent<Cottage>();
                LayerTile l1 = Map.map.GetTileByWorldPosition(cottage.wo.GetCenteredPosition());
                LayerTile l2 = Map.map.GetTileByWorldPosition(wo.GetCenteredPosition());

                res = !cottage.UnderConstruction() &&
                     cottage.GetCount() < cottage.maxOccupation && !cottage.GetPeople().Contains(wo) &&
                     wo.player == cottage.wo.player && Map.map.Adjacent(l1, l2) &&
                     (wo.GetAction() == ActionEnum.Exit || wo.GetAction() == ActionEnum.Idle);
            }
            return res;
            
        }

        public void Act(WorldObject other)
        {
            EnterCottage(other);
        }
        public CommandEnum GetCommand()
        {
            return CommandEnum.EnterCottage;
        }

        public bool CanShowButton(WorldObject otherWO)
        {
            return otherWO != null && 
                otherWO.GetWoName() == "Cottage" && 
                otherWO.GetAction() != ActionEnum.Build && 
                otherWO.player == wo.player;
        }

        public string GetCommandName(WorldObject otherWO)
        {
            return "Enter Cottage";
        }
    }
}
