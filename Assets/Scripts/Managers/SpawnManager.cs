using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    public void Spawn(GameObject objectPrefab, Vector3 position, bool isForPlacement)
    {
        if (isForPlacement)
        {
            // Logic for placing a building
            GameObject building = Instantiate(objectPrefab, position, Quaternion.identity);
            Debug.Log($"Placed building: {objectPrefab.name} at position {position}");
        }
    }

    public GameObject SpawnHighlightObjects(string poolKey, Vector3 position)
    {
        GameObject obj = PoolManager.Instance.GetObjectFromPool(poolKey);

        // pull object from pool and place in world
        if (obj != null)
        {
            obj.transform.position = position;
            obj.SetActive(true);
            return obj;
        }
        

        Debug.LogWarning("PoolReturnedNull - Max objects for pool reached or pool isnt working properly");
        return null;
    }
    public GameObject SpawnSoldierFromPool(string poolKey, Vector2 gridPos, Vector2 buildingSize)
    {
        var PosToSpawnAtGrid = getSpawnableAreaForSelected(gridPos, buildingSize);

        if (PosToSpawnAtGrid == new Vector2Int(-1, -1))
        {
            Debug.LogWarning("NoSpawnableSpaces");
            return null;
        }
       
        var PosToSpawnAtInWorld = GridManager.Instance.GridToWorldPosition(PosToSpawnAtGrid);
        GameObject obj = PoolManager.Instance.GetObjectFromPool(poolKey);
        // fill grid position 
        GridManager.Instance.SetTile(PosToSpawnAtGrid, obj);
        // pull object from pool and place in world
        if (obj != null)
        {
            obj.transform.position = PosToSpawnAtInWorld;
            obj.SetActive(true);
            return obj;
        }


        Debug.LogWarning("PoolReturnedNull - Max objects for pool reached or pool isnt working properly");
        return null;
    }

    public void ReturnToPool(string poolKey, GameObject obj)
    {
        PoolManager.Instance.ReturnObjectToPool(poolKey, obj);
    }

    private Vector2Int getSpawnableAreaForSelected(Vector2 gridPos,Vector2 buildingSize)
    {
        List<Vector2> spawnablePositions = new List<Vector2>();

        // Start at bottom-left corner outside the building
        Vector2 startPos = new Vector2(gridPos.x, gridPos.y - 1);

        // Step sizes
        int width = (int)buildingSize.x;
        int height = (int)buildingSize.y;

        // Bottom side (left to right)
        for (int x = 0; x <= width; x++)
        {
            spawnablePositions.Add(new Vector2(gridPos.x + x, gridPos.y - 1));
        }

        // Right side (bottom to top)
        for (int y = 0; y <= height; y++)
        {
            spawnablePositions.Add(new Vector2(gridPos.x + width , gridPos.y + y));
        }

        // Top side (right to left)
        for (int x = width ; x >= 0; x--) 
        {
            spawnablePositions.Add(new Vector2(gridPos.x + x, gridPos.y + height ));
        }

        // Left side (top to bottom)
        for (int y = height ; y >= -1; y--) 
        {
            spawnablePositions.Add(new Vector2(gridPos.x-1 , gridPos.y + y));
        }
        for (int i = 0; i < spawnablePositions.Count; i++)
        {
            if (GridManager.Instance.IsValidGridPosition(Vector2Int.RoundToInt(spawnablePositions[i])))
            {
                return Vector2Int.RoundToInt(spawnablePositions[i]);
            }
        }
        return new Vector2Int(-1,-1);   // -1-1 is not a valid grid. this will be the edge case since vector2 cant be nulled.
    }
}
