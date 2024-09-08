using Unity.VisualScripting;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    
    private int gridWidth = 48;
    private int gridHeight = 24;
    private float tileSize = 0.32f;

    private Tile[,] _grid;

    private void Start()
    {
        GenerateGrid();
        
    }
    public float GetTileSize()
    {
        return tileSize;
    }
    private void GenerateGrid()
    {
        _grid = new Tile[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y);
                Tile tileData = new Tile(gridPosition.x, gridPosition.y);
                _grid[x, y] = tileData;
                _grid[x, y].ClearOccupancy();
            }
        }
        GridVisualiser visualiser = FindObjectOfType<GridVisualiser>();
        visualiser.Visualise(_grid);
    }

    //--------------- GRID GENERATION UP TO HERE ---------------\\

    //--------------- GRID FUNCTIONS FROM HERE ON ---------------\\
    public Tile GetTile(Vector2Int gridPosition)
    {
        if (gridPosition.x < 0 || gridPosition.x > gridWidth - 1 || gridPosition.y < 0 || gridPosition.y > gridHeight - 1)
            return null;       // if out of bounds return null
        return _grid[gridPosition.x, gridPosition.y];
    }

    public Vector2Int WorldPositionToGrid(Vector2 worldPosition)
    {
        // Round instead of floor for more accurate placement
        int x = Mathf.RoundToInt((worldPosition.x / tileSize) + gridWidth / 2);
        int y = Mathf.RoundToInt((worldPosition.y / tileSize) + gridHeight / 2);

        return new Vector2Int(x, y);
    }

    public Vector2 GridToWorldPosition(Vector2Int gridPosition)
    {
        return new Vector2(
            (gridPosition.x - gridWidth / 2) * tileSize,
            (gridPosition.y - gridHeight / 2) * tileSize
        );
    }


    public bool IsValidGridPosition(Vector2Int gridPosition)
    {
        if (gridPosition.x < 0 || gridPosition.x > gridWidth - 1 || gridPosition.y < 0 || gridPosition.y > gridHeight - 1)
            return false; // if out of bounds, return false

        return !_grid[gridPosition.x, gridPosition.y].IsOccupied; // return false if the tile is occupied
    }


    public void SetTile(Vector2Int gridPosition,GameObject occuppant)
    {
        _grid[gridPosition.x,gridPosition.y].SetOccupied(occuppant);
    }

    public Tile ReturnClosestEmptyTile(Vector2Int targetPos)
    {
        Tile closestTile = null;
        float closestDistance = float.MaxValue;

        // Loop through all grid positions to find the closest empty tile
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Tile tile = _grid[x, y];

                if (!tile.IsOccupied) // Only consider unoccupied tiles
                {
                    float distance = Vector2.Distance(new Vector2(x, y), targetPos);

                    if (distance < closestDistance)
                    {
                        closestTile = tile;
                        closestDistance = distance;
                    }
                }
            }
        }

        if (closestTile != null)
        {
            Debug.Log("Found closest empty tile at: " + closestTile.GridPosition + " Distance: " + closestDistance);
        }
        else
        {
            Debug.LogWarning("No empty tile found.");
        }

        return closestTile;
    }

}
