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
        Tile startTile = gridManager.GetTile(start);
        Tile goalTile = gridManager.GetTile(goal);

        // Debugging
        Debug.Log($"Start Tile: {start}, Goal Tile: {goal}");
        Debug.Log($"StartTile is Occupied: {startTile.IsOccupied}, GoalTile is Occupied: {goalTile.IsOccupied}");

        // Ignore the start tile's occupancy since the unit is already there
        if (startTile == null)
        {
            Debug.LogError($"Start tile is null at position {start}");
            return null;
        }

        if (goalTile == null)
        {
            Debug.LogError($"Goal tile is null at position {goal}");
            return null;
        }

        // Goal tile can be occupied if it's the same as start tile (unit is already on it)
        if (goalTile.IsOccupied && goalTile != startTile)
        {
            Debug.LogError($"Goal tile is occupied by another object at {goal}");
            return null;
        }

        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();
        Dictionary<Tile, (int GCost, int HCost, Tile Parent)> pathData = new Dictionary<Tile, (int, int, Tile)>();

        openSet.Add(startTile);
        pathData[startTile] = (0, GetHeuristic(startTile, goalTile), null);  // GCost = 0, HCost = heuristic to goal, no parent yet

        while (openSet.Count > 0)
        {
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
                Debug.Log("Path found.");
                return ReconstructPath(pathData, currentTile);  // Path found
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            foreach (Tile neighbor in GetNeighbors(currentTile))
            {
                if (neighbor.IsOccupied && neighbor != goalTile)
                {
                    continue; // Skip occupied tiles unless it's the goal
                }

                if (closedSet.Contains(neighbor))
                {
                    continue;
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
                pathData[neighbor] = (tentativeGCost, GetHeuristic(neighbor, goalTile), currentTile);  // Parent is currentTile
            }
        }

        Debug.LogError("No valid path found.");
        return null;  // No path found
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
        new Vector2Int(0, -1), // Down
        new Vector2Int(1, 1),  // Up-Right (Diagonal)
        new Vector2Int(-1, 1), // Up-Left (Diagonal)
        new Vector2Int(1, -1), // Down-Right (Diagonal)
        new Vector2Int(-1, -1) // Down-Left (Diagonal)
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
