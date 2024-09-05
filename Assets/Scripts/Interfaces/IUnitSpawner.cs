using UnityEngine;

public interface IUnitSpawner
{
    UnitStats[] ProduceableUnits ();

    void CheckInitialisePool();
}
