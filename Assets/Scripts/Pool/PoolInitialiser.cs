using UnityEngine;

public class PoolInitialiser : MonoBehaviour
{
    [SerializeField] GameObject itemstoPool;
    private void Start()
    {
        PoolManager.Instance.CreatePool("CantPlaceObject",itemstoPool,6);
    }
}
