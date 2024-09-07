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

        // Check if start and goal are valid tiles
        if (startTile == null || goalTile == null)
        {
            Debug.LogError("Invalid start or goal tile!");
            return null;
        }

        // Allow moving to the goal even if occupied by an enemy
        // If the goal is occupied, move to the nearest free tile
        if (goalTile.IsOccupied && goalTile != gridManager.GetTile(gridManager.WorldPositionToGrid(this.transform.position)))
        {
            Debug.Log("Goal tile is occupied, moving to closest tile.");
            return FindClosestFreeTile(goalTile);
        }

        List<Tile> openSet = new List<Tile> { startTile };
        HashSet<Tile> closedSet = new HashSet<Tile>();
        Dictionary<Tile, (float GCost, float HCost, Tile Parent)> pathData = new Dictionary<Tile, (float, float, Tile)>
        {
            [startTile] = (0f, GetDistance(startTile, goalTile), null)
        };

        while (openSet.Count > 0)
        {
            Tile currentTile = GetTileWithLowestFCost(openSet, pathData);

            if (currentTile == goalTile)
            {
                Debug.Log("Path found!");
                return ReconstructPath(pathData, goalTile);
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            foreach (Tile neighbor in GetNeighbours(currentTile))  // Only cardinal neighbors
            {
                if (closedSet.Contains(neighbor) || (neighbor.IsOccupied && neighbor != goalTile))
                    continue;

                float tentativeGCost = pathData[currentTile].GCost + GetDistance(currentTile, neighbor);

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeGCost >= pathData[neighbor].GCost)
                {
                    continue;
                }

                pathData[neighbor] = (tentativeGCost, GetDistance(neighbor, goalTile), currentTile);
            }
        }

        Debug.LogError("No valid path found.");
        return null;
    }

    private List<Vector2Int> FindClosestFreeTile(Tile goalTile)
    {
        List<Vector2Int> closestTiles = new List<Vector2Int>();



        return closestTiles;
    }

    private List<Tile> GetNeighbours(Tile tile)
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


    private List<Vector2Int> ReconstructPath(Dictionary<Tile, (float GCost, float HCost, Tile Parent)> pathData, Tile goalTile)
    {
        List<Vector2Int> path = new List<Vector2Int> { goalTile.GridPosition };

        Tile currentTile = goalTile;
        while (pathData[currentTile].Parent != null)
        {
            currentTile = pathData[currentTile].Parent;
            path.Add(currentTile.GridPosition);
        }

        path.Reverse(); // Reverse to get the path from start to goal
        return path;
    }



    private Tile GetTileWithLowestFCost(List<Tile> openSet, Dictionary<Tile, (float GCost, float HCost, Tile Parent)> pathData)
    {
        Tile lowestTile = openSet[0];
        float lowestFCost = pathData[lowestTile].GCost + pathData[lowestTile].HCost;

        foreach (Tile tile in openSet)
        {
            float fCost = pathData[tile].GCost + pathData[tile].HCost;
            if (fCost < lowestFCost || (fCost == lowestFCost && pathData[tile].HCost < pathData[lowestTile].HCost))
            {
                lowestTile = tile;
                lowestFCost = fCost;
            }
        }

        return lowestTile;
    }

    private float GetDistance(Tile a, Tile b)
    {
        float dx = Mathf.Abs(a.GridPosition.x - b.GridPosition.x);
        float dy = Mathf.Abs(a.GridPosition.y - b.GridPosition.y);
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

}
