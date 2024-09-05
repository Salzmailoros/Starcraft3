using UnityEngine;

public class Barracks : BuildingBase, IClickable, IDamageable, IUnitSpawner
{
    [SerializeField] private BuildingStats barracksStats;

    private void Start()
    {
        Initialize(barracksStats);
        GetComponent<SpriteRenderer>().sortingOrder = 1;
    }
    public UnitStats[] ProduceableUnits()
    {
        return barracksStats.ProduceableUnits;
    }

    void IDamageable.Die()
    {
        Die();
    }

    void IClickable.OnLeftClick()
    {
        Debug.Log($" Selected Building of type : {stats.buildingName}");
        // update the UI to show this buildings menu/stats/stuff
    }

    void IClickable.OnRightClick()
    {
        Debug.Log($"Targeted Building : {stats.buildingName}");
    }

    void IUnitSpawner.CheckInitialisePool()
    {
        var unitlist = barracksStats.ProduceableUnits;
        for (int i = 0; i < unitlist.Length; i++)
        {
            UnitStats unit = unitlist[i];

            // Check if the pool exists
            if (!PoolManager.Instance.DoesPoolExist(unit.name))
            {
                // create pool if not.
                PoolManager.Instance.CreatePool(unit.name, unit.unitPrefab, 10);
            }
        }
    }

}