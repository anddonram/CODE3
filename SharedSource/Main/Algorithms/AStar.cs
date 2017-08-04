using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.TiledMap;

namespace Codigo.Algorithms
{
    public class AStar
    {

        public static List<LayerTile> Astar(LayerTile start, LayerTile end)
        {
            FogOfWar fog = Map.map.Owner.FindComponent<FogOfWar>();
            //the adjacency neighbor list
            List<Point> adjPoints = new List<Point>();
            adjPoints.Add(new Point(0, 1));
            adjPoints.Add(new Point(1, 0));
            adjPoints.Add(new Point(-1, 0));
            adjPoints.Add(new Point(0, -1));

            // The set of nodes already evaluated.
            HashSet<LayerTile> closedSet = new HashSet<LayerTile>();

            // For each node, which node it can most efficiently be reached from.
            // If a node can be reached from many nodes, cameFrom will eventually contain the
            // most efficient previous step.
            Dictionary<LayerTile, LayerTile> cameFrom = new Dictionary<LayerTile, LayerTile>();

            // For each node, the cost of getting from the start node to that node.
            // map with default value of Infinity
            Dictionary<LayerTile, float> gScore = new Dictionary<LayerTile, float>();

            // The cost of going from start to start is zero.
            gScore[start] = 0;

            // For each node, the total cost of getting from the start node to the goal
            // by passing by that node. That value is partly known, partly heuristic.
            // map with default value of Infinity
            Dictionary<LayerTile, float> fScore = new Dictionary<LayerTile, float>();

            // For the first node, that value is completely heuristic.
            fScore[start] = HeuristicCostEstimate(start, end);

            // The set of currently discovered nodes that are not evaluated yet.
            // Initially, only the start node is known.
            // The set is ordered by the fScore

            SortedList<LayerTile, float> openSet = new SortedList<LayerTile, float>(new AStarTileComparer(fScore));
            openSet.Add(start, fScore[start]);


            while (openSet.Count > 0)
            {
                LayerTile current = openSet.ElementAt(0).Key;//the node in openSet having the lowest fScore[] value
                if (current == end)
                    return ReconstructPath(cameFrom, current);
                openSet.RemoveAt(0);
                //if (!)
                //{
                //    //this should not be happening, so this is an error and we exit
                //    Trace.WriteLine(openSet.Contains(current));
                //    return new List<LayerTile>();
                //}
                closedSet.Add(current);
                foreach (Point neighborPoint in fog.Adjacents(current, adjPoints))
                {
                    LayerTile neighbor = Map.map.GetTileByMapCoordinates(neighborPoint.X, neighborPoint.Y);
                    if (closedSet.Contains(neighbor) ||
                        (Map.map.IsTileOccupied(neighborPoint.X, neighborPoint.Y) && fog.IsVisible(neighbor)))
                        continue;       // Ignore the neighbor which is already evaluated. Or we can see it is not traversable

                    if (!gScore.ContainsKey(neighbor))
                    {
                        gScore[neighbor] = float.PositiveInfinity;
                    }
                    float tentative_gScore = gScore[current] + DistBetween(current, neighbor,fog);
                    if (!openSet.ContainsKey(neighbor)) // Discover a new node
                        openSet.Add(neighbor, gScore[neighbor] + HeuristicCostEstimate(neighbor, end));
                    else if (tentative_gScore >= gScore[neighbor])
                        continue;       // This is not a better path.

                    // This path is the best until now. Record it!
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentative_gScore;
                    fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, end);
                    openSet[neighbor] = fScore[neighbor];
                }
            }
            return new List<LayerTile>();
        }

        private static float DistBetween(LayerTile current, LayerTile neighbor,FogOfWar fog)
        {
            float dist = float.PositiveInfinity;
            if (fog.IsVisible(current) && fog.IsVisible(neighbor))
            {
                if (!Map.map.IsTileOccupied(neighbor.X, neighbor.Y))
                {
                    dist = 1;
                }
            }
            else if (fog.IsVisible(current) && !fog.IsVisible(neighbor))
            {
                dist = 3;
            }
            else if (!fog.IsVisible(current) && fog.IsVisible(neighbor))
            {
                if (!Map.map.IsTileOccupied(neighbor.X, neighbor.Y))
                    dist = 2;
            }
            else
            {
                dist = 5;
            }
            return dist;
        }

        private static List<LayerTile> ReconstructPath(Dictionary<LayerTile, LayerTile> cameFrom, LayerTile current)
        {
            List<LayerTile> totalPath = new List<LayerTile>();
            totalPath.Add(current);

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Add(current);
            }
            //this returns the path in reverse order, we must reverse the list
            totalPath.Reverse();
            return totalPath;
        }

        private static float HeuristicCostEstimate(LayerTile start, LayerTile end)
        {
            float X = (start.X - end.X);
            float Y = (start.Y - end.Y);
            //abs(x)+abs(y)
            return (X < 0 ? -X : X) + (Y < 0 ? -Y : Y);
        }

    }
}
