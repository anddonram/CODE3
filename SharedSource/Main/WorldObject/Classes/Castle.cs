using Codigo.Components;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace Codigo
{
    public class Castle:Component
    {
        private WorldObject[] castleWO;
        private int size = 0;
        public Player player;

        public int GetSize()
        {
            return size;
        }
        public bool SetCastle(int xStart, int yStart, int width, int height,Player active)
        {
            
            Map map = Map.map;
            if (map.InBounds(xStart, yStart) && map.InBounds(xStart+width-1,yStart+height-1)) {
                size = width * height;
                WorldObject[] parts = new WorldObject[size];
                int index = 0;
                for (int i = xStart; i < xStart + width; i++)
                {
                    for(int j = yStart; j < yStart + height; j++)
                    {
                        if (map.GetWorldObject(i,j)!=null)
                        {
                            size--;
                        }
                        else
                        {
                            parts[index] = new WorldObject();
                            Entity ent = new Entity()
                                .AddComponent(parts[index]);
                            parts[index].SetWoName("Castle");
                            parts[index].SetTraversable(true);
                            parts[index].SetMobile(false);
                            parts[index].SetMaxHealth(200);
                            parts[index].player=active;
                            map.SetWorldObject(i, j, parts[index]);
                            
                            // for now, as this is traversable, we don't occupy the tile
                           // map.SetTileOccupied(i, j, true);
                            ent
                                .AddComponent(new SpriteRenderer())
                                .AddComponent(new Transform2D
                                {
                                    Position = map.GetTilePosition(i, j),DrawOrder=1f
                                })
                                .AddComponent(new Sprite(WaveContent.Assets.road_PNG))
                                .AddComponent(new WorldObjectTraits())
                                .AddComponent(new FogRevealBehavior())
                                .AddComponent(new FogAdjacents());
                            Owner.AddChild(ent);
                            
                            index++;
                        }
                    }
                }
                castleWO = new WorldObject[size];
                Array.Copy(parts, castleWO, size);
                
            }
            return size > 0;
        }
        public void DestroyPart(WorldObject castlePart)
        {
            for(int i = 0; i < castleWO.Length; i++)
            {
                if (castlePart == castleWO[i]) { castleWO[i] = null; }
            }
            if (IsDestroyed()) { Destroy(); }
        }
        private bool IsDestroyed()
        {
            bool destroyed = true;
            foreach (WorldObject wo in castleWO) {
                if (wo != null&&!wo.IsDestroyed())
                {
                    destroyed = false;
                    break;
                }
            }
            return destroyed;
        }
        private void Destroy()
        {
            player.castle = null;
            EntityManager.Remove(Owner);
            
        }
    }
}
