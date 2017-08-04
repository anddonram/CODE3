using Codigo.Algorithms;
using Codigo.Behaviors;
using Codigo.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.TiledMap;

namespace Codigo
{
    /**
     * <summary>
     * This class computes paths using D*Lite variant. Requires a fog of war component, otherwise it wont work
     * </summary>
     */
    public class DStarLite : Behavior
    {

        private FogAdjacents fa;

        private FogOfWar fog;
        private Map map;

        [RequiredComponent]
        private WorldObject wo;
        [RequiredComponent]
        private MovementBehavior move;

        public Dictionary<LayerTile, float> gScore;
        public Dictionary<LayerTile, float> rhs;
        public Dictionary<LayerTile, Vector2> fScore;
        Dictionary<LayerTile, LayerTile> cameFrom;

        SortedList<LayerTile, Vector2> openSet;

        private float km;

        public List<Point> adjPoints { get; private set; }

        private LayerTile end;
        private LayerTile start;
        private LayerTile last;

        private HashSet<LayerTile> updatedTiles;
        /**
         * <summary>
         * If we punish a little for visiting previous tiles, we may avoid some loops
         * </summary>
         */
        Dictionary<LayerTile, int> visitedTiles;
        /**
         * <summary>
         * When we compute the first path, this is set to true until it stops
         * </summary>
         */
        public bool pathSet = false;

        public DStarLite()
        {
            map = Map.map;
            fog = FogOfWar.fog;
            //the adjacency neighbor list
            adjPoints = new List<Point>();
            adjPoints.Add(new Point(0, 1));
            adjPoints.Add(new Point(1, 0));
            adjPoints.Add(new Point(-1, 0));
            adjPoints.Add(new Point(0, -1));

        }
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            fa = Owner.FindComponent<FogAdjacents>(false);
        }
        public void Reset()
        {
            gScore.Clear();
            rhs.Clear();
            fScore.Clear();
            cameFrom.Clear();
            openSet.Clear();

            start = null;
            end = null;
            last = null;
            km = 0;
            recalculate = false;
            pathSet = false;

            updatedTiles.Clear();
            visitedTiles.Clear();

        }
        protected override void DefaultValues()
        {
            base.DefaultValues();

            // For each node, the cost of getting from the start node to that node.
            // map with default value of Infinity
            gScore = new Dictionary<LayerTile, float>();

            rhs = new Dictionary<LayerTile, float>();


            // For each node, the total cost of getting from the start node to the goal
            // by passing by that node. That value is partly known, partly heuristic.
            // map with default value of Infinity
            fScore = new Dictionary<LayerTile, Vector2>();

            // For each node, which node it can most efficiently be reached from.
            // If a node can be reached from many nodes, cameFrom will eventually contain the
            // most efficient previous step.
            cameFrom=new Dictionary<LayerTile, LayerTile>();
            
            // The set of currently discovered nodes that are not evaluated yet.
            // The set is ordered by the fScore
            openSet = new SortedList<LayerTile, Vector2>(new DStarTileComparer(fScore));

            start = null;
            end = null;
            last = null;
            km = 0;

            pathSet = false;

            updatedTiles=new HashSet<LayerTile>();
            visitedTiles=new Dictionary<LayerTile, int>();
            recalculate = false;
        }
        private Vector2 CalculateKey(LayerTile t)
        {
            if (!gScore.ContainsKey(t))
            {
                gScore[t] = float.PositiveInfinity;
            }
            if (!rhs.ContainsKey(t))
            {
                rhs[t] = float.PositiveInfinity;
            }
            return new Vector2(Math.Min(gScore[t], rhs[t]) + HeuristicCostEstimate(start, t) + km,
                Math.Min(gScore[t], rhs[t]));
        }
        /**
         * <summary>
         * This loads the first things
         * </summary>
         */
        public List<LayerTile> DStar(LayerTile startTile, LayerTile endTile)
        {
            Reset();
            start = startTile;
            end = endTile;
            last = start;
            // The cost of going from end to end is zero.
            rhs[end] = 0;

            // For the first node, that value is completely heuristic.
            fScore[end] = CalculateKey(end);

            // Initially, only the start node is known.
            openSet.Add(end, fScore[end]);

            

            ComputeShortestPath();
            List<LayerTile> tiles= ReconstructPath();
            pathSet = true;
            return tiles;
        }

        private void UpdateVertex(LayerTile u)
        {
            if (!gScore.ContainsKey(u))
            {
                gScore[u] = float.PositiveInfinity;
            }
            if (u != end)
            {
                float minVal = float.PositiveInfinity;
                foreach (Point neighborPoint in fog.Adjacents(u, adjPoints))
                {
                    LayerTile s = map.GetTileByMapCoordinates(neighborPoint.X, neighborPoint.Y);
                    if (!gScore.ContainsKey(s))
                    {
                        gScore[s] = float.PositiveInfinity;
                    }
                    float currVal= (DistBetween(u, s)) + gScore[s];
                    if (minVal > currVal)
                    {
                        minVal = currVal;
                    }
                }
                rhs[u] = minVal;
            }
            if (openSet.ContainsKey(u))
            {
                openSet.Remove(u);
            }
            if (gScore[u] != rhs[u])
            {
                fScore[u] = CalculateKey(u);
                openSet.Add(u,fScore[u]);
            }
        }
        
        private bool CompareKeys(Vector2 fx,Vector2 fy)
        {
            int comp = fx.X.CompareTo(fy.X);
            if (comp == 0)
            {
                comp = fx.Y.CompareTo(fy.Y);   
            }
            return comp < 0;
        }
        /**
         * <summary>
         * This calculates a new initial path between start and end.
         * </summary>
         */
        private void ComputeShortestPath()
        {

            while (openSet.Count>0&&(CompareKeys(openSet.First().Value, CalculateKey(start)) ||rhs[start]!=gScore[start]))
            {
                fScore[start] = CalculateKey(start);
                Vector2 oldKey = openSet.First().Value;

                LayerTile u = openSet.First().Key;//the node in openSet having the lowest fScore[] value
       
                openSet.Remove(u);
                if (CompareKeys(oldKey, CalculateKey(u)))
                {
                    fScore[u] = CalculateKey(u);
                    openSet.Add(u, fScore[u]);
                } else if (gScore[u] > rhs[u])
                {
                    gScore[u] = rhs[u];
                    foreach (Point neighborPoint in fog.Adjacents(u, adjPoints))
                    {
                        LayerTile s = map.GetTileByMapCoordinates(neighborPoint.X, neighborPoint.Y);
                        UpdateVertex(s);
                    }
                }
                else {
                    gScore[u] = float.PositiveInfinity;
                    UpdateVertex(u);
                    foreach (Point neighborPoint in fog.Adjacents(u, adjPoints))
                    {
                        LayerTile s = map.GetTileByMapCoordinates(neighborPoint.X, neighborPoint.Y);
                        UpdateVertex(s);
                    }
                    
                }
 
            }

        }

        private float DistBetween(LayerTile current, LayerTile neighbor)
        {
            float dist = 1;

            //If neighbor revealed or visible and occupied or not traversable, no path
            if (neighbor != start && neighbor != end)
                dist = CalculateDistance(neighbor);

            //If current revealed or visible and occupied or not traversable, no path
            if (dist != float.PositiveInfinity && current != start && current != end)
                dist = CalculateDistance(current);

            //If current not revealed nor visible and neighbor not revealed nor visible, dist is unknown
            if (dist != float.PositiveInfinity && fog.IsNotVisible(neighbor) && fog.IsNotVisible(current))
                dist = 3;
            return dist;
        }

        public float CalculateDistance(LayerTile tile)
        {
            float dist = visitedTiles.ContainsKey(tile) ? visitedTiles[tile] : 1;
            if (fog.IsVisible(tile))
            {
                //Tile visible, we check real objects
                WorldObject obs = map.GetWorldObject(tile.X, tile.Y);
                WorldObject mob = map.GetMobile(tile.X, tile.Y);
                if (map.IsTileOccupied(tile.X, tile.Y) ||
                    (obs != null && !obs.IsTraversable(wo)) ||
                    (mob != null && !mob.IsTraversable(wo)))
                {
                    dist = float.PositiveInfinity;
                }
            }
            else if (fog.IsPartiallyVisible(tile))
            {
                //partial fog, we check only revealed entity
                WorldObject obs = fog.GetRevealedWO(tile.X, tile.Y);
                if (obs != null && !obs.IsTraversable(wo))
                {
                    dist = float.PositiveInfinity;
                }
            }

            return dist;
        }

        private List<LayerTile> ReconstructPath()
        {
            List<LayerTile> totalPath = new List<LayerTile>();
            if (gScore[start] == float.PositiveInfinity)
            {
                Reset();
                return totalPath; 
            }

            LayerTile current = start;
            totalPath.Add(current);
            while (current!=end)
            {
                float minVal = float.PositiveInfinity;
                LayerTile nextTile = null;
                foreach (Point neighborPoint in fog.Adjacents(current, adjPoints))
                {
                    LayerTile s = map.GetTileByMapCoordinates(neighborPoint.X, neighborPoint.Y);
                    float currVal = DistBetween(current, s) + gScore[s];
                    if (rhs.ContainsKey(s)&&minVal > rhs[s])
                    {
                        minVal = rhs[s];
                        nextTile = s;
                    }
                }
                current = nextTile;
                totalPath.Add(current);

            }
            // totalPath.Reverse();
            return totalPath;
        }

        public float HeuristicCostEstimate(LayerTile start, LayerTile end)
        {
            float X = (start.X - end.X);
            float Y = (start.Y - end.Y);
            //abs(x)+abs(y)
            return (X < 0 ? -X : X) + (Y < 0 ? -Y : Y);
        }
        /**
         * <summary>
         * This is true when we have changed tile, so in the next frame we will compute a new path.
         * We need that frame lapse so all the proper tiles are added to updatedTiles
         * </summary>
         */
        private bool recalculate = false;
        protected override void Update(TimeSpan gameTime)
        {
            if (pathSet)
                if (start != end)
                {
                    if (wo.GetAction() == ActionEnum.Move) { 
                        updatedTiles.UnionWith(fog.updatedTiles);
                        if (gScore[start] == float.PositiveInfinity || move.FullStop())
                        {
                            //there is not a known path
                            Reset();
                        }
                        else
                        {
                            LayerTile tile = Map.map.GetTileByWorldPosition(wo.GetCenteredPosition());
                            if (recalculate)
                            {
                                recalculate = false;
                                float minVal = float.PositiveInfinity;
                                LayerTile nextTile = null;
                                foreach (Point neighborPoint in fog.Adjacents(start, adjPoints))
                                {
                                    LayerTile s = map.GetTileByMapCoordinates(neighborPoint.X, neighborPoint.Y);
                                    if (minVal > rhs[s])
                                    {
                                        minVal = rhs[s];
                                        nextTile = s;
                                    }
                                }

                                start = nextTile;

                                km += HeuristicCostEstimate(last, start);
                                last = start;
                                foreach (LayerTile s in updatedTiles)
                                {
                                    foreach (Point neighborPoint in fog.Adjacents(s, adjPoints))
                                    {
                                        LayerTile u = map.GetTileByMapCoordinates(neighborPoint.X, neighborPoint.Y);
                                        UpdateVertex(u);
                                    }
                                    UpdateVertex(s);
                                }
                                updatedTiles.Clear();
                                ComputeShortestPath();
                                List<LayerTile> tiles = ReconstructPath();
                              
                                move.ForcePath(tiles);

                            }
                            else if (last != tile)
                            {
                                //If we already visited this tile, we punish going back this way each time harder
                                if (visitedTiles.ContainsKey(last))
                                {
                                    visitedTiles[last] = visitedTiles[last] * 3;
                                }
                                else
                                {
                                    visitedTiles.Add(last, 1);
                                }
                                recalculate = true;
                                recalculate = true;
                            }
                        }
                    }
                }
                else Reset();
        }
  
    }
}
