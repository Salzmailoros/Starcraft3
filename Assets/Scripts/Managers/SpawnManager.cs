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
        else
        {
            // Logic for producing units (e.g., soldiers) from a building
            GameObject unit = Instantiate(objectPrefab, position, Quaternion.identity);
            Debug.Log($"Produced unit: {objectPrefab.name} at position {position}");
        }
    }

    public void Spawn(UnitStats soldierData, Vector3 spawnPosition)
    {
        // Logic for producing soldiers
        GameObject soldier = Instantiate(soldierData.unitPrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"Produced soldier: {soldierData.unitPrefab.name} at position {spawnPosition}");
    }

    public GameObject SpawnFromPool(string poolKey, Vector3 position, Quaternion rotation)
    {
        GameObject obj = PoolManager.Instance.GetObjectFromPool(poolKey);
        if (obj != null)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            return obj;
        }
        return null;
    }

    public void ReturnToPool(string poolKey, GameObject obj)
    {
        PoolManager.Instance.ReturnObjectToPool(poolKey, obj);
    }
}
