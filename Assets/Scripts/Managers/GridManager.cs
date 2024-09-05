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
    }

    //--------------- GRID GENERATION UP TO HERE ---------------\\

    //--------------- GRID FUNCTIONS FROM HERE ON ---------------\\
    public Tile GetTile(Vector2Int gridPosition)
    {
        if (IsValidGridPosition(gridPosition))
        {
            return _grid[gridPosition.x, gridPosition.y];
        }
        return null;
    }

    public Vector2Int WorldPositionToGrid(Vector2 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / tileSize) + gridWidth / 2;
        int y = Mathf.FloorToInt(worldPosition.y / tileSize) + gridHeight / 2;

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
        if (gridPosition.x < 0 || gridPosition.x > gridWidth-1 || gridPosition.y < 0 || gridPosition.y > gridHeight-1)
            return false;       // if out of bounds return false
        else
        return !_grid[gridPosition.x, gridPosition.y].IsOccupied;
    }

    public void SetTile(Vector2Int gridPosition,GameObject occuppant)
    {
        _grid[gridPosition.x,gridPosition.y].SetOccupied(occuppant);
    }
}
