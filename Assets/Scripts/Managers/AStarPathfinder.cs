using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder : Singleton<AStarPathfinder>
{
    private GridManager gridManager;

    protected override void Awake()
    {
        base.Awake();
        gridManager = GridManager.Instance;
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();

        // Dictionary that holds GCost, HCost, and Parent together
        Dictionary<Tile, (int GCost, int HCost, Tile Parent)> pathData = new Dictionary<Tile, (int, int, Tile)>();

        Tile startTile = gridManager.GetTile(start);
        Tile goalTile = gridManager.GetTile(goal);

        if (startTile == null || goalTile == null)
        {
            Debug.LogError("Invalid start or goal position.");
            return null;
        }

        openSet.Add(startTile);
        pathData[startTile] = (0, GetHeuristic(startTile, goalTile), null);  // GCost = 0, HCost = heuristic to goal, no parent yet

        while (openSet.Count > 0)
        {
            // Find the tile with the lowest FCost (GCost + HCost)
            Tile currentTile = openSet[0];
            int lowestFCost = pathData[currentTile].GCost + pathData[currentTile].HCost;

            foreach (Tile tile in openSet)
            {
                var tileData = pathData[tile];
                int fCost = tileData.GCost + tileData.HCost;

                if (fCost < lowestFCost || (fCost == lowestFCost && tileData.HCost < pathData[currentTile].HCost))
                {
                    currentTile = tile;
                    lowestFCost = fCost;
                }
            }

            if (currentTile == goalTile)
            {
                // Path found
                return ReconstructPath(pathData, currentTile);
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            foreach (Tile neighbor in GetNeighbors(currentTile))
            {
                if (neighbor.IsOccupied && neighbor != goalTile)
                {
                    continue; // Skip occupied tiles (except goal)
                }

                if (closedSet.Contains(neighbor))
                {
                    continue; // Ignore tiles already evaluated
                }

                int tentativeGCost = pathData[currentTile].GCost + GetDistance(currentTile, neighbor);

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeGCost >= pathData[neighbor].GCost)
                {
                    continue; // This path is not better
                }

                // Update path data for the neighbor
                pathData[neighbor] = (tentativeGCost, GetHeuristic(neighbor, goalTile), currentTile);
            }
        }

        // No path found
        return null;
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Tile, (int GCost, int HCost, Tile Parent)> pathData, Tile currentTile)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        path.Add(currentTile.GridPosition);

        while (pathData[currentTile].Parent != null)
        {
            currentTile = pathData[currentTile].Parent;
            path.Add(currentTile.GridPosition);
        }

        path.Reverse();
        return path;
    }


    private int GetHeuristic(Tile a, Tile b)
    {
        // Using Manhattan distance as heuristic
        return Mathf.Abs(a.GridPosition.x - b.GridPosition.x) + Mathf.Abs(a.GridPosition.y - b.GridPosition.y);
    }

    private int GetDistance(Tile a, Tile b)
    {
        // Assuming uniform cost between adjacent tiles
        return 1;
    }

    private List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        Vector2Int[] directions = {
            new Vector2Int(1, 0),  // Right
            new Vector2Int(-1, 0), // Left
            new Vector2Int(0, 1),  // Up
            new Vector2Int(0, -1)  // Down
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighborPos = tile.GridPosition + direction;
            if (gridManager.IsValidGridPosition(neighborPos))
            {
                Tile neighborTile = gridManager.GetTile(neighborPos);
                if (neighborTile != null)
                {
                    neighbors.Add(neighborTile);
                }
            }
        }

        return neighbors;
    }
}
