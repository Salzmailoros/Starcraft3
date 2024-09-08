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
        // If the start and goal are the same, return an empty path (no movement needed)
        if (start == goal)
        {
            Debug.Log("Start and goal positions are the same. No movement required.");
            return new List<Vector2Int>();  // Return an empty path
        }

        Tile startTile = gridManager.GetTile(start);
        Tile goalTile = gridManager.GetTile(goal);

        // Validate the start and goal tiles
        if (!IsValidTile(startTile) || !IsValidTile(goalTile))
        {
            Debug.LogError("Invalid start or goal tile.");
            return null;
        }

        List<Tile> openSet = new List<Tile> { startTile };
        HashSet<Tile> closedSet = new HashSet<Tile>();
        Dictionary<Tile, PathNode> pathData = new Dictionary<Tile, PathNode>
        {
            [startTile] = new PathNode(0, GetDistance(startTile, goalTile), null)
        };

        while (openSet.Count > 0)
        {
            Tile currentTile = GetTileWithLowestFCost(openSet, pathData);

            if (currentTile == goalTile)
            {
                return ReconstructPath(pathData, goalTile);
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            foreach (Tile neighbor in GetCardinalNeighbours(currentTile))  // Ensure only cardinal neighbors
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

                pathData[neighbor] = new PathNode(tentativeGCost, GetDistance(neighbor, goalTile), currentTile);
            }
        }

        return null;
    }


    // Get the valid cardinal neighbors of the tile (No diagonals)
    private List<Tile> GetCardinalNeighbours(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        // Only cardinal directions (right, left, up, down)
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
                if (neighborTile != null && !neighborTile.IsOccupied)  // Ensure tile is unoccupied
                {
                    neighbors.Add(neighborTile);
                }
            }
        }

        return neighbors;
    }

    // Reconstruct the path from the start to goal tile
    private List<Vector2Int> ReconstructPath(Dictionary<Tile, PathNode> pathData, Tile goalTile)
    {
        List<Vector2Int> path = new List<Vector2Int> { goalTile.GridPosition };
        Tile currentTile = goalTile;

        while (pathData[currentTile].Parent != null)
        {
            currentTile = pathData[currentTile].Parent;

            // Skip any unnecessary steps where the next step is the same as the current position
            if (pathData[currentTile].Parent == null) continue;

            path.Add(currentTile.GridPosition);
        }

        path.Reverse();
        return path;
    }

    private float GetDistance(Tile a, Tile b)
    {
        return Mathf.Abs(b.GridPosition.x - a.GridPosition.x) + Mathf.Abs(b.GridPosition.y - a.GridPosition.y);
    }


    // Find the tile with the lowest FCost in the open set
    private Tile GetTileWithLowestFCost(List<Tile> openSet, Dictionary<Tile, PathNode> pathData)
    {
        Tile lowestTile = openSet[0];
        float lowestFCost = pathData[lowestTile].FCost;

        foreach (Tile tile in openSet)
        {
            float fCost = pathData[tile].FCost;
            if (fCost < lowestFCost || (fCost == lowestFCost && pathData[tile].HCost < pathData[lowestTile].HCost))
            {
                lowestTile = tile;
                lowestFCost = fCost;
            }
        }

        return lowestTile;
    }

    // Validates a tile
    private bool IsValidTile(Tile tile)
    {
        return tile != null;
    }
}

// Helper class for pathfinding nodes
public class PathNode
{
    public float GCost;   // Distance from the start tile
    public float HCost;   // Heuristic: distance to the goal tile
    public Tile Parent;   // Parent tile to reconstruct the path

    public float FCost => GCost + HCost;  // Total cost (G + H)

    public PathNode(float gCost, float hCost, Tile parent)
    {
        GCost = gCost;
        HCost = hCost;
        Parent = parent;
    }
}
