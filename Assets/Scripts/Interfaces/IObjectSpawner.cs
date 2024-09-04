using UnityEngine;

public interface IObjectSpawner
{
    void ProduceUnit(GameObject prefab);
    GameObject[] ProduceableUnits ();
}
