using Codigo.Data;
using WaveEngine.Common.Math;
using WaveEngine.Framework;

namespace Codigo
{
    public abstract class WorldObjectData
    {

        public string woName { get; protected set; }
        public bool traversable { get; protected set; }
        public bool mobile { get; protected set; }
        public float maxHealth { get; protected set; }
        public float health { get; protected set; }
        public string sprite { get; protected set; }
        public float woodRequired { get; protected set; }
        public bool isBuilding { get; protected set; }

        /**
         * <summary>
         * Adds the corresponding trait and returns the same entity
         * </summary>
         */
        public virtual Entity AddTraits(Entity ent) { return ent.AddComponent(new WorldObjectTraits()); }
        /**
         * <summary>
         * Adds all required components and returns the same entity
         * </summary>
         */
        public abstract Entity AddComponents(Entity ent);
        /**
        * <summary>
        * the size of the sprite and the tiles
        * </summary>
        */
        public static readonly Vector2 tileSize = new Vector2(32, 32);
        /**
        * <summary>
        * the center of the tile, to decide whether have we changed tile or not
        * </summary>
        */
        public static readonly Vector2 center = tileSize / 2; 

        /**
         * <summary>
         * These WOs can be created by the player
         * </summary>
         */
        public static readonly CommandEnum[] buildings = {CommandEnum. Candle,
            CommandEnum.Trap,
            CommandEnum.FakeTree,
            CommandEnum.Cottage,
            CommandEnum.Bridge };

        /**
         * <summary>
         * These WOs are placed by the game, or by a debugger
         * </summary>
         */
        public static readonly CommandEnum[] notBuildings = { CommandEnum.Rock,
        CommandEnum.Tree,
         CommandEnum.Castle,
         CommandEnum.Person,
         CommandEnum.Water    };

        public static WorldObjectData GetData(string name)
        {
            WorldObjectData data = null;
            switch (name)
            {
                case "Person":
                    data = new PersonData();
                    break;
                case "Castle":
                    data = new CastleData();
                    break;
                case "Tree":
                    data = new TreeData();
                    break;
                case "Rock":
                    data = new RockData();
                    break;
                case "Trap":
                    data = new TrapData();
                    break;
                case "FakeTree":
                    data = new FakeTreeData();
                    break;
                case "Cottage":
                    data = new CottageData();
                    break;
                case "Candle":
                    data = new CandleData();
                    break;
                case "Water":
                    data = new WaterData();
                    break;
                case "Bridge":
                    data = new BridgeData();
                    break;
                default:
                    break;
            }
            return data;
        }
    }

}
