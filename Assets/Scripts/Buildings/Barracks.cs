using UnityEngine;

public class Barracks : BuildingBase, IClickable, IDamageable, IUnitSpawner
{
    [SerializeField] private BuildingStats barracksStats;

    public int TeamID = 0;

    int IDamageable.TeamID { get => TeamID; set => TeamID = value; }

    private void Awake()
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
        base.Die();
    }

    void IClickable.OnLeftClick()
    {
        //Debug.Log($" Selected Building of type : {buildingStats.buildingName}");
    }

    void IClickable.OnRightClick()
    {
        //Debug.Log($"Targeted Building : {buildingStats.buildingName}");
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