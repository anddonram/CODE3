using Codigo.Components;
using System;
using System.Diagnostics;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.TiledMap;

namespace Codigo.Behaviors
{
    public class FightBehavior : Behavior,ActionBehavior
    {
        private Boolean cooldownON;
        [RequiredComponent]
        private WorldObject wo;
        [RequiredComponent(false)]
        private FightTraits fightTraits;

        /**
         * <summary>
         * Who are we fighting with
         * </summary>
         */
        private WorldObject rival;

   
        protected override void Update(TimeSpan gameTime)
        {
            if (wo.attacking && wo.GetAction() == ActionEnum.Idle)
            {
                Trace.WriteLine("Searching for an enemy...");
                WorldObject aux = IsSomeoneAdjacent();
                if (aux != null)
                {
                    SetRival(aux);
                }
            }
            if (wo.GetAction() == ActionEnum.Fight)
            {
                Fight();
            }
        }
        /**
          * <summary>
          * Fights an adjacent enemy
          * </summary>
          */
        private void Fight()
        {

            if (fightTraits!=null && fightTraits.CanAttack())
            {
                if (rival != null && !cooldownON)
                    if (!rival.IsDestroyed() && rival.GetAction() != ActionEnum.Inside && wo.IsAdjacent(rival))
                    {
                        Boolean destroyed = rival.TakeDamage(fightTraits.GetAttack());

                        fightTraits.AfterAttack();
                        if (destroyed)
                        {
                            rival = null;
                            wo.Stop();
                        }
                        cooldownON = true;

                        WaveServices.TimerFactory.CreateTimer(TimeSpan.FromSeconds(fightTraits.GetAttackSpeed()), () => { cooldownON = false; }).Looped = false;
                    }
                    else
                    {
                        rival = null;
                        wo.Stop();
                    }
            }else
            {
                rival = null;
                wo.Stop();
            }
        }
        /**
          * <summary>
          * starts fighting a WO 
          * </summary>
          * <param name="riv">
          * the wo to be fought
          * </param>
          */
        public void SetRival(WorldObject riv)
        {
            //You can only attack people, cottages, castles, traps and bridges
            if (CanAct(riv))
            {
                rival = riv;
                wo.SetAction(ActionEnum.Fight);
            }
        }



        /**
          * <summary>
          * returns the world object which is adjacent to this and has less health (if any)
          * </summary>
          */
        private WorldObject IsSomeoneAdjacent()
        {
            Map map = Map.map;
            WorldObject res= null;
            LayerTile tile = map.GetTileByWorldPosition(this.Owner.FindComponent<Transform2D>().Position);
            if (tile == null)
            {
                
                return res;
            }
            else
            {
                if (map.InBounds(tile.X + 1, tile.Y))
                {
                    if (map.GetMobile(tile.X + 1, tile.Y) != null && map.GetMobile(tile.X + 1, tile.Y).player!=this.wo.player)
                    {
                        res = map.GetMobile(tile.X + 1, tile.Y);
                    }
                }
                if (map.InBounds(tile.X - 1, tile.Y))
                {
                    if (map.GetMobile(tile.X - 1, tile.Y) != null && map.GetMobile(tile.X - 1, tile.Y).player != this.wo.player)
                    {
                        if (res != null)
                        {
                            if (res.GetHealth() > map.GetMobile(tile.X - 1, tile.Y).GetHealth())
                            {
                                res = map.GetMobile(tile.X - 1, tile.Y);
                            }

                        }
                        else
                        {
                            res = map.GetMobile(tile.X - 1, tile.Y);
                        }

                    }
                }
                if (map.InBounds(tile.X, tile.Y + 1))
                {
                    if (map.GetMobile(tile.X, tile.Y + 1) != null && map.GetMobile(tile.X, tile.Y + 1).player != this.wo.player)
                    {
                        if (res != null)
                        {
                            if (res.GetHealth() > map.GetMobile(tile.X, tile.Y + 1).GetHealth())
                            {
                                res = map.GetMobile(tile.X, tile.Y + 1);
                            }

                        }
                        else
                        {
                            res = map.GetMobile(tile.X, tile.Y + 1);
                        }

                    }
                }
                if (map.InBounds(tile.X, tile.Y - 1))
                {
                    if (map.GetMobile(tile.X, tile.Y - 1) != null && map.GetMobile(tile.X, tile.Y - 1).player != this.wo.player)
                    {
                        if (res != null)
                        {
                            if (res.GetHealth() > map.GetMobile(tile.X, tile.Y - 1).GetHealth())
                            {
                                res = map.GetMobile(tile.X, tile.Y - 1);
                            }

                        }
                        else
                        {
                            res = map.GetMobile(tile.X, tile.Y - 1);
                        }
                    }
                }

            }
            if (res != null)
            {
            }
            return res;
        }

        public bool CanAct(WorldObject other)
        {
            return fightTraits != null && 
                fightTraits.CanAttack() && 
                other != null &&
                !other.IsDestroyed()&&
                !wo.IsSameTeam(other.player) && 
                !wo.IsActionBlocking() && 
                other.IsAttackable(wo);
        }

        public void Act(WorldObject other)
        {
            SetRival(other);
        }
        public CommandEnum GetCommand()
        {
            return CommandEnum.Fight;
        }

        public bool CanShowButton(WorldObject otherWO)
        {
            return otherWO != null && !otherWO.IsSameTeam(wo.player) && otherWO.IsAttackable(wo);
        }

        public string GetCommandName(WorldObject otherWO)
        {
            return string.Format("Fight {0}", otherWO.GetWoName());
        }
    }
}
