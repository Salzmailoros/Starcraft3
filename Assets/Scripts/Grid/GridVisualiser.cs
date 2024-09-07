using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualiser : MonoBehaviour
{
    public GameObject gridprefab;
    public bool visualiseGrid;

    public void Visualise(Tile[,] grid)
    {
        if (visualiseGrid)
        {
            foreach (Tile tile in grid)
            {
                Instantiate(gridprefab, new Vector3(GridManager.Instance.GridToWorldPosition(tile.GridPosition).x, GridManager.Instance.GridToWorldPosition(tile.GridPosition).y, 0), Quaternion.identity, transform);
            }
        }
       
    }

}
