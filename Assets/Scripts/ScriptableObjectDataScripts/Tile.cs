
using UnityEngine;

namespace Game.Data
{
    public class Tile
    {
        public Vector2Int GridPosition { get; private set; }
        public bool IsOccupied { get; set; }
        public GameObject Occupant { get; set; }

        public Tile(int x, int y)
        {
            GridPosition = new Vector2Int(x, y);
            IsOccupied = false;
            Occupant = null;
        }
    }
}
