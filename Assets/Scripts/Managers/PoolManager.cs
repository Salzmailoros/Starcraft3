using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<string, ObjectPool> _pools = new Dictionary<string, ObjectPool>();

    public void CreatePool(string poolKey, GameObject prefab, int initialSize)
    {
        if (!_pools.ContainsKey(poolKey))
        {
            // Create a new parent GameObject for this pool within the scene
            GameObject poolObject = new GameObject(poolKey);

            poolObject.transform.SetParent(null);  // Be a regular object not a destroyonload.
            

            // Create a new object pool and add it to the dictionary
            ObjectPool pool = new ObjectPool(prefab, initialSize, poolObject.transform,poolKey);
            _pools.Add(poolKey, pool);
        }
    }

    public GameObject GetObjectFromPool(string poolKey)
    {
        if (_pools.TryGetValue(poolKey, out var pool))
        {
            GameObject obj = pool.Get();
            if (obj == null)
            {
                // If the pool is exhausted, instantiate a new object
                obj = pool.CreateNewObject();
            }
            return obj;
        }

        Debug.LogWarning($"Pool with key '{poolKey}' not found.");
        return null;
    }

    public void ReturnObjectToPool(string poolKey, GameObject obj)
    {
        if (_pools.TryGetValue(poolKey, out var pool))
        {
            pool.ReturnToPool(obj);
        }
        else
        {
            Debug.LogWarning($"Pool with key '{poolKey}' not found.");
            Destroy(obj);
        }
    }

    public bool DoesPoolExist(string poolKey)
    {
        return _pools.ContainsKey(poolKey);
    }
}
