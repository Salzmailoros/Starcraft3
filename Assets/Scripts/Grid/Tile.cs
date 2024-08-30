using UnityEngine;

public class Tile
{
    public Vector2Int GridPosition { get; private set; }
    public bool IsOccupied { get; private set; }
    public GameObject OccupyingObject { get; private set; }

    public Tile(int x, int y)
    {
        GridPosition = new Vector2Int(x, y);
        IsOccupied = false;
    }

    public void SetOccupied(GameObject occupant)
    {
        IsOccupied = true;
        OccupyingObject = occupant;
    }

    public void ClearOccupancy()
    {
        IsOccupied = false;
        OccupyingObject = null;
    }
}