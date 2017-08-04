using Codigo.Behaviors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.TiledMap;

namespace Codigo.Components
{
    public class Cottage : Building
    {
        public const  int MAX_OCCUPATION= 6;
        private List<WorldObject> people;
        /**
         * <summary>
         * How many people the cottage can shelter
         * </summary>
         */
        public int maxOccupation { get; private set; } = MAX_OCCUPATION;
        private int count;

        protected override void Initialize()
        {
            base.Initialize();
            count = 0;
            people = new List<WorldObject>();
        }

        public List<WorldObject> GetPeople()
        {
            return people;
        }
        public void AddPerson(WorldObject wo)
        {
            if (!UnderConstruction() && !people.Contains(wo))
            {
                people.Add(wo);
            }
        }

        public bool IsDestroyed()
        {
            return wo.IsDestroyed();
        }
        /**
         * <summary>
         * How many people is strictly inside the cottage
         * </summary>
         */
        public int GetOccupation()
        {
            return people.Count;
        }
        /**
         * <summary>
         * How many people is in the cottage OR is entering
         * </summary>
         */
        public int GetCount()
        {
            return count;
        }
        public void SetCount(int count)
        {
            this.count = count;
        }
        public void ExitPerson(int position, LayerTile tile)
        {
            if (tile != null && GetOccupation() >= position && position > 0 &&
                !(Map.map.IsTileOccupied(tile.X, tile.Y) ||
                Map.map.GetWorldObject(tile.X, tile.Y) != null &&
                (!Map.map.GetWorldObject(tile.X, tile.Y).IsTraversable(people[position - 1]))) &&
                Map.map.Adjacent(tile, Map.map.GetTileByWorldPosition(wo.transform.Position)))
            {
                WorldObject person = people[position - 1];

                people.Remove(person);
                SetCount(GetCount() - 1);
                Trace.WriteLine(GetCount());
                if (!person.IsDestroyed())
                {
                    person.Owner.FindComponent<EnterExitBehavior>().ExitCottage(Map.map.GetTileByWorldPosition(wo.transform.Position), tile);
                }
            }
        }

        protected override void Removed()
        {
            //Destroy all persons inside
            base.Removed();
            if (people != null)
                foreach (WorldObject wo in people)
                {
                    if (wo != null && !wo.IsDestroyed())
                    {
                        wo.Destroy();
                    }
                }
            people = null;
        }
    }
}
