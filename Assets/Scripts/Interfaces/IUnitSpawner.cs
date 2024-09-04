using UnityEngine;

public interface IUnitSpawner
{
    void ProduceUnit(GameObject prefab);
    UnitStats[] ProduceableUnits ();
}
