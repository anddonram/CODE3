using Codigo.Components;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.TiledMap;

namespace Codigo
{
    public class FogOfWar : Component
    {
        /**
         * <summary>
         * Static access to unique fog instance
         * </summary>
         */
        public static FogOfWar fog;
        private const int OFF = 0, ON = 1, PLAYER = 2;
  
        private Map map;

        private int state = ON;

        private SpriteRenderer[,] quads;

        private int[,] visibility;
        /**
         * <summary>
         * In this matrix we store the objects that must be shown in the semi revealed parts of the map.
         * Only for not mobile WOs. Mobile WOs should be invisible if not in the line of vision of the active player units.
         * </summary>
         */
        private PartialFog[,] revealed;

        /**
         * <summary>
         * The list of tiles whose visibility have changed in the last two frames. D* purposes.
         * </summary>
         */
        public List<LayerTile> updatedTiles;
        /**
         * <summary>
         * The number of tiles added during this frame, so we can know 
         * which of them delete from updatedTiles list (those from the previous frame).
         * </summary>
         */
        public int tilesAdded;

        public void InitializeFog(bool on)
        {
            updatedTiles= new List<LayerTile>();
            this.map = Map.map;
            quads = new SpriteRenderer[map.width, map.height];
            visibility = new int[map.width, map.height];
            revealed = new PartialFog[map.width,map.height];
            state = on ? ON : OFF;
            for (int i = 0; i < quads.GetLength(0); i++)
                for (int j = 0; j < quads.GetLength(1); j++)
                {
                    if (map.InBounds(i, j))
                    {
                        LayerTile tile = map.GetTileByMapCoordinates(i, j);

                        revealed[i, j] = new PartialFog();

                        Entity ent = new Entity() {IsStatic=true };
                        ent.Name = ent.Name + "Fog";
                        quads[i, j] = new SpriteRenderer { IsVisible=on,LayerType=DefaultLayers.Alpha};
                        ent.AddComponent(new Transform2D { Position = tile.LocalPosition })
                            .AddComponent(quads[i, j])
                            .AddComponent(new Sprite(WaveContent.Assets.cloud_PNG) );
                        EntityManager.Add(ent);
                        visibility[i , j] = 0;

                        //add the partial visibility components
                        Entity entRev = new Entity();
                        entRev.Name = ent.Name + "PartFog";
                        entRev.AddComponent(new Transform2D { Position = tile.LocalPosition,DrawOrder=0.5f })
                           .AddComponent( new SpriteRenderer ())
                           .AddComponent(new Sprite())
                           .AddComponent(revealed[i,j]);
                        EntityManager.Add(entRev);
                    }
                }
        }
        /**
         * <summary>
         * This enables and disables fog for a player, keeping the status
         * </summary>
         */
        public void ToggleFog(bool on)
        {
            if (state == PLAYER)
            {

                for (int i = 0; i < quads.GetLength(0); i++)
                    for (int j = 0; j < quads.GetLength(1); j++)
                    {
                        quads[i, j].IsVisible = on && visibility[i, j] == 0;
                    }
            }
        }
        /**
         * <summary>
         * This enables and disables fog completely, reseting the status
         * </summary>
         */
        public void TurnFog(bool on)
        {
            if ((state == ON && !on) || (state == OFF && on) || state == PLAYER)
            {
                
                state = on ? ON : OFF;
                for (int i = 0; i < quads.GetLength(0); i++)
                    for (int j = 0; j < quads.GetLength(1); j++)
                    {
                        quads[i, j].IsVisible = on;
                        quads[i, j].Sprite.MaterialPath = null;
                        visibility[i, j] = 0;
                        ResetRevealedSprite(i, j);
                    }
            }

        }
        /**
         * <summary>
         * Changes visibility for the new player, reseting the status
         * </summary>
         */
        public void SetFogForPlayer(Player p)
        {
            if (p == null) {
                TurnFog(false);
            }
            else
            {
                state = PLAYER;
                TurnFog(true);
                state = PLAYER;
            }
   
        }
        /**<summary>
         * Reveals visibility. Used on creation
         * </summary>
         */
        public void SetUp(Vector2 position, List<Point> adjacents)
        {
            LayerTile tile = map.GetTileByWorldPosition(position);
            List<Point> tiles = Adjacents(tile, adjacents);
            foreach (Point t in tiles)
            {
                //make the older tiles  visible, thus disabling clouds
                TurnTile(t.X, t.Y, true);
                AddUpdatedTile(map.GetTileByMapCoordinates(t.X, t.Y));
            }
        }
        /**<summary>
         * Reveals visibility. Used on destruction
         * </summary>
         */
        public void Erase(Vector2 position, List<Point> adjacents)
        {
            LayerTile tile = map.GetTileByWorldPosition(position);
            List<Point> tiles = Adjacents(tile,adjacents);
            foreach (Point t in tiles)
            {
                //make the older tiles not visible, thus enabling clouds
                TurnTile(t.X, t.Y, false);
                AddUpdatedTile(map.GetTileByMapCoordinates(t.X, t.Y));
            }

        }
        /**
         * <summary>
         * Given the current position, the last position and the list of adjacents, update map visibility
         * </summary>
         */
        public void TurnTiles(Vector2 position, Vector2 lastPosition, List<Point> adjacents)
        {
            LayerTile tile = map.GetTileByWorldPosition(position);
            LayerTile lastTile = map.GetTileByWorldPosition(lastPosition);
            if (tile != lastTile)
            {
                List<Point> oldTiles = Adjacents(lastTile, adjacents);
                List<Point> newTiles = Adjacents(tile, adjacents);
                foreach (Point t in oldTiles)
                {
                    //make the older tiles not visible, thus enabling clouds
                    if (!newTiles.Contains(t))
                        TurnTile(t.X, t.Y, false);
                    AddUpdatedTile(map.GetTileByMapCoordinates(t.X, t.Y));
                }

                foreach (Point t in newTiles)
                {
                    //make the new tiles visible, thus disabling clouds

                    AddUpdatedTile(map.GetTileByMapCoordinates(t.X, t.Y));
                    if (!oldTiles.Contains(t))
                        TurnTile(t.X, t.Y, true);
                    
                }
            }
        }
        /**
         * <summary>
         * Turns the points into a list of tiles relative to the tile given
         * </summary>
         */
        public List<Point> Adjacents(LayerTile tile, List<Point> adjacents)
        {
            if (tile != null)
            {
                Point tilePos = new Point(tile.X, tile.Y);
                return adjacents
                     .ConvertAll(adj => Sum(tilePos, adj))
                     .FindAll(vec => InBounds(vec));
            }
            return new List<Point>();
        }
        /**<summary>
         * Sums two points
         * </summary>
         */
        private Point Sum(Point x, Point y)
        {
            return new Point(x.X + y.X, x.Y + y.Y);
        }
        /**
         * <summary>
         * Given a point, whether there is a tile in the map with that coordinates
         * </summary>
         */
        private bool InBounds(Point tileCoordinates)
        {
            return map.InBounds(tileCoordinates.X,tileCoordinates.Y);
        }
        /**
         * <summary>
         * Does all stuff regarding one tile, including partial visibility and revealing tiles and what is inside them
         * </summary>
         */
        private void TurnTile(int x, int y, bool tileVisible)
        {
            
                AddUpdatedTile(map.GetTileByMapCoordinates(x, y));
                if (tileVisible)
                {
                    visibility[x, y]++;
                    if (visibility[x, y] == 1)
                    {
                        //tile visible, so cloud not visible
                        quads[x, y].IsVisible = false;

                        //disable the previous revealed entity
                        revealed[x, y].Owner.IsVisible = false;

                        //show the object
                        WorldObject wo = map.GetWorldObject(x, y);
                        if (wo != null && wo.IsVisible(UIBehavior.ui.activePlayer))
                        {
                            wo.Owner.IsVisible = true;
                        }
                        
                        //show the mobile
                        WorldObject mobile = map.GetMobile(x, y);
                        if (mobile != null && mobile.IsVisible(UIBehavior.ui.activePlayer))
                        {
                            mobile.Owner.IsVisible = true;
                        }
                    }
                }
                else
                {
                    visibility[x, y]--;
                    if (visibility[x, y] <= 0)
                    {
                        //tile not visible, so cloud visible
                        visibility[x, y]=0;
                        quads[x, y].IsVisible = true;
                        
                        //now it is partially visible
                        if (quads[x, y].Sprite.MaterialPath == null)
                            quads[x, y].Sprite.MaterialPath = WaveContent.Assets.TransparentMaterial;
                        //Create the temporary object for partial visibility
                        WorldObject wo = map.GetWorldObject(x, y);
                        revealed[x, y].Owner.IsVisible = true;
                        if (wo == null || wo.IsDestroyed())
                        {
                            ResetRevealedSprite(x, y);
                        }
                        else
                        {
                            //hide the real wo 
                            wo.Owner.IsVisible = false;
                            if (revealed[x, y].originalEntity != wo)
                            {
                                //and replace its sprite
                                if (wo.IsVisible(UIBehavior.ui.activePlayer))
                                {
                                    //if we could see it
                                    revealed[x, y].originalEntity = wo;
                                    revealed[x, y].sprite.TexturePath = wo.Owner.FindComponent<Sprite>().TexturePath;
                                }
                                else
                                {
                                    //if we could not see(p.e. traps) we reset the pseudo sprite
                                    ResetRevealedSprite(x, y);
                                }
                            }
                        }
                        //hide the mobile
                        WorldObject mobile = map.GetMobile(x, y);
                        if (mobile!=null) {
                            mobile.Owner.IsVisible = false;
                        }
                    }
                }       
        }
        /**<summary>
         * Clears what we can see in the tile due to partial fog 
         * </summary>
         */
        private void ResetRevealedSprite(int x,int y)
        {
            revealed[x, y].originalEntity = null;
            revealed[x, y].sprite.TexturePath = null;
        }
        /**
         * <summary>
         * returns whether the tile is totally visible
         * </summary>
         */
        public bool IsVisible(LayerTile tile) {
            return tile != null && IsVisible(tile.X,tile.Y);
        }
        /**
         * <summary>
         * returns whether the tile is partially visible
         * </summary>
         */
        public bool IsPartiallyVisible(LayerTile tile)
        {
            return tile != null && IsPartiallyVisible(tile.X, tile.Y);
        }
        /**
         * <summary>
         * returns whether the tile is not visible (covered by fog)
         * </summary>
         */
        public bool IsNotVisible(LayerTile tile)
        {
            return tile != null && IsNotVisible(tile.X, tile.Y);
        }
        /**
         * <summary>
         * returns whether the tile in the coordinates is totally visible
         * </summary>
         */
        public bool IsVisible(int x,int y)
        {
            return map.InBounds(x,y)&& !quads[x, y].IsVisible;
        }
        /**
         * <summary>
         * returns whether the tile in the coordinates has been revealed but it's not visible
         * </summary>
         */
        public bool IsPartiallyVisible(int x,int y)
        {
            return map.InBounds(x, y) && quads[x, y].IsVisible && quads[x, y].Sprite.MaterialPath != null;
        }
        /**
         * <summary>
         * returns whether the tile in the coordinates is totally covered by the fog (not visible)
         * </summary>
         */
        public bool IsNotVisible(int x, int y)
        {
            return !map.InBounds(x, y) || (quads[x, y].IsVisible && quads[x, y].Sprite.MaterialPath == null);
        }
        /**
         * <summary>
         * returns the fake world object at the specified coordinates, only for partial visibility
         * </summary>
         */
        public WorldObject GetRevealedWO(int x,int y)
        {
            return IsPartiallyVisible(x, y) ? revealed[x, y].originalEntity : null;
        }
        /**
         * <summary>
         * Adds the tile to the list of changed visibility. All D* will take them into account
         * </summary>
         */
        public void AddUpdatedTile(LayerTile tile)
        {
            if (!updatedTiles.Contains(tile))
            {
                tilesAdded++;
                updatedTiles.Add(tile);
            }
        }
    }
}
