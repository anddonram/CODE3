using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.TiledMap;

namespace Codigo.Behaviors
{
    public class SwitchBehavior : Component,ActionBehavior
    {

        [RequiredComponent]
        private WorldObject wo;


        /// <summary>
        /// The other wo we are switching
        /// </summary>
        private SwitchBehavior other;
        private LayerTile thisTile, otherTile;

        /**
         * <summary>
         * Moves both mobiles half the way between the two tiles
         * </summary>
         */
        public void Switch(WorldObject other)
        {

            if (CanAct(other))
            {
                SwitchBehavior beh = other.Owner.FindComponent<SwitchBehavior>();
                //calculate halfway
                Vector2 halfway = (beh.wo.transform.Position + wo.transform.Position) / 2;

                //for this person, switch
                this.other = beh;
                thisTile = Map.map.GetTileByWorldPosition(wo.GetCenteredPosition());
                otherTile = Map.map.GetTileByWorldPosition(beh.wo.GetCenteredPosition());

                wo.SetAction(ActionEnum.Switch);

                wo.animation = new WaveEngine.Components.GameActions.MoveTo2DGameAction(Owner, halfway, TimeSpan.FromSeconds(wo.genericSpeed / 2));
                wo.animation.Completed += SwapPlaces;
                wo.animation.Run();

                //for the other person, switch 
                beh.other = this;
                beh.otherTile = Map.map.GetTileByWorldPosition(wo.GetCenteredPosition());
                beh.thisTile = Map.map.GetTileByWorldPosition(beh.wo.GetCenteredPosition());
                this.other.wo.SetAction(ActionEnum.Switch);
                this.other.wo.animation = new WaveEngine.Components.GameActions.MoveTo2DGameAction(beh.Owner, halfway, TimeSpan.FromSeconds(wo.genericSpeed / 2));
                this.other.wo.animation.Completed += this.other.SwapPlaces;
                this.other.wo.animation.Run();
            }



        }
        /**
         * <summary>
         * This method swaps places in the map at halfway, then continue to the end of the tile
         * </summary>
         */
        private void SwapPlaces(IGameAction obj)
        {

            if (other.wo.IsDestroyed())
            {
                //If the other was destroyed, release the previous tile and occupy the new
                Map.map.SetMobile(wo.GetX(), wo.GetY(), null);
                Map.map.SetTileOccupied(wo.GetX(), wo.GetY(), false);
                Map.map.SetTileOccupied(otherTile.X, otherTile.Y, true);
            }
            //If we are moving to a non traversable area, like water because the bridge was destroyed, go back.
            if(Map.map.GetWorldObject(otherTile.X, otherTile.Y) != null && !Map.map.GetWorldObject(otherTile.X, otherTile.Y).IsTraversable(this.wo))
            {       
                Owner.FindComponent<MovementBehavior>().MoveBack(thisTile, otherTile);
                other = null;
                otherTile = null;
                thisTile = null;
            }
            else
            {
                Map.map.SetMobile(otherTile.X, otherTile.Y, wo);
                LayerTile tile = Map.map.GetTileByMapCoordinates(wo.GetX(), wo.GetY());
                var animation = new WaveEngine.Components.GameActions.MoveTo2DGameAction(Owner, tile.LocalPosition, TimeSpan.FromSeconds(wo.genericSpeed / 2));
                animation.Completed += Stop;
                animation.Run();
            }
        }

        private void Stop(IGameAction obj)
        {
            other = null;
            otherTile = null;
            thisTile = null;
            wo.Stop();
        }

        public bool CanAct(WorldObject other)
        {
            bool res = other != null && !other.IsDestroyed();
            if (res)
            {
                SwitchBehavior beh = other.Owner.FindComponent<SwitchBehavior>();
                res = beh != null && !wo.IsActionBlocking() && !beh.wo.IsActionBlocking() && beh.wo.player == this.wo.player && wo.IsAdjacent(beh.wo);
            }
            return res;
        }

        public void Act(WorldObject other)
        {
            Switch(other);
        }
        public CommandEnum GetCommand()
        {
            return CommandEnum.Switch;
        }

        public bool CanShowButton(WorldObject otherWO)
        {
            return otherWO != null && otherWO.IsMobile() && otherWO.player == wo.player;
        }

        public string GetCommandName(WorldObject otherWO)
        {
            return "Switch tiles";
        }
    }
}
