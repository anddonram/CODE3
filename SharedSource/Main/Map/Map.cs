using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Materials;
using WaveEngine.TiledMap;

namespace Codigo
{
    public class Map : Component
    {
        public static Map map;

        [RequiredComponent]
        public TiledMap tiledMap { get; private set; }

        /**
         * <summary>
         * width of the playable map
         * </summary>
         */
        public int width { get; private set; }
        /**
         * <summary>
        * height of the playable map
        * </summary>
        */
        public int height { get; private set; }
    
        /**
        * <summary>
        * Whether the tile is occupied or not, and traversable
        * </summary>
         */
        private bool[,] occupied;

        /**
         * <summary>
         * The mobile that is in the tile
         * </summary>
         */
        private WorldObject[,] mobiles;
        /**
         * <summary>
         * The object that is in the tile, usually these are traversable
         * </summary>
          */
        private WorldObject[,] objects;
        /**
         * <summary>
         * the first layer of the map, from which we obtain all the tiles
         * </summary>
         */
        private TiledMapLayer mapLayer;

        protected override void Initialize()
        {
            base.Initialize();
            SetBounds(8, 12);
        }
        /**
         * <summary>
         * Sets the bounds of the playable fields. Must be called before SetMap()
         * </summary>
         */
        private void SetBounds(int width, int height)
        {
            int tiledWidth = tiledMap.Width;
            int tiledHeight = tiledMap.Height;
            if (height <= 0 ||  height > tiledHeight )
            {
                this.height = tiledHeight;
            }
            else
            {
                this.height = height;
            }
            if ( width <= 0  || width > tiledWidth)
            {
                this.width = tiledWidth;
            }
            else
            {
                this.width = width;
            }
            Trace.WriteLine(this.height + ", " + this.width);
        }
        /**
         * <summary>
         * Loads the map from the TiledMap into the arrays
         * </summary>
         */
        public void SetMap()
        {
            IEnumerator<string> keys = tiledMap.TileLayers.Keys.GetEnumerator();
            if (keys.MoveNext())
            {
                string layer = keys.Current;

                occupied = new bool[width, height];

                mobiles = new WorldObject[width, height];
                objects = new WorldObject[width, height];

                if (layer != null)
                {
                    mapLayer = tiledMap.TileLayers[layer];
                    foreach (LayerTile tile in mapLayer.Tiles)
                    {

                        if (InBounds(tile.X, tile.Y))
                        {
                            occupied[tile.X, tile.Y] = false;
                            mobiles[tile.X, tile.Y] = null;
                            objects[tile.X, tile.Y] = null;
                        }

                    }
                }
            }

            Color back = RenderManager.ActiveCamera2D.BackgroundColor;
            //Hide non-playable map
            if (height != tiledMap.Height)
            {

                Entity quad = new Entity()
                   .AddComponent(new Transform2D()
                   {
                       Position = new Vector2(width * tiledMap.TileWidth, 0),
                      
                       Scale = new Vector2((tiledMap.Width - width) , tiledMap.Height )
                   })
                   
                    .AddComponent(new Sprite(WaveContent.Assets.Blank_png) { TintColor = back })
               .AddComponent(new SpriteRenderer());
                EntityManager.Add(quad);
            }
            if (width != tiledMap.Width)
            {
                Entity quad = new Entity()
               .AddComponent(new Transform2D()
               {
                   Position = new Vector2(0, height * tiledMap.TileHeight),
                    Scale = new Vector2(tiledMap.Width , (tiledMap.Height - height) )
               })
               
               .AddComponent(new Sprite(WaveContent.Assets.Blank_png) {  TintColor = back })
               .AddComponent(new SpriteRenderer());
                EntityManager.Add(quad);

            }

        }

        public bool IsTileOccupied(int x, int y)
        {
            return occupied[x,y];
        }
        public void SetTileOccupied(int x,int y,bool occupy)
        {
            if (InBounds(x, y))
            {
                occupied[x, y] = occupy;
            }
        }
        public Vector2 GetTilePosition(int x, int y)
        {
            return GetTileByMapCoordinates(x, y).LocalPosition;
        }


        public WorldObject GetMobile(int x, int y)
        {
            return mobiles[x, y];
        }

        public void SetMobile(int x, int y, WorldObject wo)
        {
            if (InBounds(x, y))
            {
                mobiles[x, y] = wo;
                if (wo != null)
                {
                    wo.SetX(x);
                    wo.SetY(y);
                }
            }
        }
        public WorldObject GetWorldObject(int x, int y)
        {
            return objects[x, y];
        }

        public void SetWorldObject(int x, int y, WorldObject wo)
        {
            if (InBounds(x, y))
            {
                objects[x, y] = wo;
                if (wo != null)
                {
                    wo.SetX(x);
                    wo.SetY(y);
                }
            }
        }
        /**
         * <summary>
         * Returns true if the coordinates are inside the playable field
         * </summary>
         */
        public bool InBounds(int x, int y) { return x >= 0 && x < width && y >= 0 && y < height; }


        /**<summary>
        * Returns true if the coordinates are inside the tiledMap bounds, 
        * which can be larger than the playable field
        * </summary>
        */
        public bool InTiledBounds(int x, int y) { return x >= 0 && x < tiledMap.Width && y >= 0 && y < tiledMap.Height; }

        /**
         * <summary>
         * Returns true if the tile is inside the playable field
         * </summary>
         */
        public bool InBounds(LayerTile tile) { return tile != null ? InBounds(tile.X, tile.Y) : false; }
        /**<summary>
         * Returns the tile at the specified position in the tiled map
         * </summary>
         */
        private LayerTile GetLayerTileByWorldPosition(Vector2 position)
        {
            return mapLayer.GetLayerTileByWorldPosition(position);
        }

        /**<summary>
         * Returns the tile at the specified position in the playable map
         * </summary>
         * <returns>
         * The tile is null if the position is outside the playable field. Always check if null before doing things with this
         * </returns>
         */
        public LayerTile GetTileByWorldPosition(Vector2 position)
        {
            LayerTile tile = GetLayerTileByWorldPosition(position);
            return InBounds(tile) ? tile : null;
        }
        /**<summary>
         * Returns the tile at the specified coordinates in the playable map
         * </summary>
         * <returns>
         * The tile is null if the position is outside the playable field. Always check if null before doing things with this
         * </returns>
         */
        public LayerTile GetTileByMapCoordinates(int x, int y)
        {
            LayerTile tile = mapLayer.GetLayerTileByMapCoordinates(x, y);
            return InBounds(tile) ? tile : null;
        }
        /**
         * <summary>
         * Checks if tiles are adjacent
         * </summary>
         */
        public Boolean Adjacent(LayerTile tile1, LayerTile tile2)
        {
            if (tile1 == null || tile2 == null)
            {
                return true;
            }
            else
            {
                return ((tile1.X == tile2.X) && (tile1.Y == (tile2.Y + 1))) || ((tile1.X == tile2.X) && (tile1.Y == (tile2.Y - 1))) ||
                    ((tile1.Y == tile2.Y) && (tile1.X == (tile2.X + 1))) || ((tile1.Y == tile2.Y) && (tile1.X == (tile2.X - 1)));
            }
        }
    }
}
