using System.Collections.Generic;
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

    public GameObject SpawnFromPool(string poolKey,Vector2 gridPos,Vector2 buildingSize,bool shouldFillSlots)
    {
        //pool
        GameObject obj = PoolManager.Instance.GetObjectFromPool(poolKey);

        //calculate the objects position in world and match the grid.
        var PosToSpawnAtGrid = getSpawnableAreaForSelected(gridPos, buildingSize);
        var PosToSpawnAtInWorld = GridManager.Instance.GridToWorldPosition(PosToSpawnAtGrid);
        // fill the correct grid position.
        if (shouldFillSlots)
        {
            GridManager.Instance.SetTile(PosToSpawnAtGrid, obj);
        }
        // pull object from pool and place in world
        if (obj != null)
        {
            obj.transform.position = PosToSpawnAtInWorld;
            obj.transform.rotation = Quaternion.identity;
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
            spawnablePositions.Add(new Vector2(gridPos.x + width + 1, gridPos.y + y));
        }

        // Top side (right to left)
        for (int x = width; x >= 0; x--)
        {
            spawnablePositions.Add(new Vector2(gridPos.x + x, gridPos.y + height + 1));
        }

        // Left side (top to bottom)
        for (int y = height; y >= 0; y--)
        {
            spawnablePositions.Add(new Vector2(gridPos.x - 1, gridPos.y + y));
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
