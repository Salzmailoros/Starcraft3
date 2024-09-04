using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private GameObject prefab;
    private Transform parentTransform;
    private Queue<GameObject> pool;
    private string myKey;

    public ObjectPool(GameObject prefab, int initialSize, Transform parentTransform,string key)
    {
        myKey = key;
        this.prefab = prefab;
        this.parentTransform = parentTransform;
        pool = new Queue<GameObject>();
        
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = CreateNewObject();

            pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return null; 
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(parentTransform);  // Re-parent the object to the pool's parent
        pool.Enqueue(obj);
    }

    public GameObject CreateNewObject()
    {
        GameObject obj = Object.Instantiate(prefab);
        obj.GetComponent<PoolableObject>().SetKey(myKey);
        obj.transform.SetParent(parentTransform); 
        obj.SetActive(false);
        return obj;
    }
}
