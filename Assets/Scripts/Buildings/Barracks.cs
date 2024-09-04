using UnityEngine;

public class Barracks : BuildingBase, IClickable, IDamageable, IUnitSpawner
{
    [SerializeField] private BuildingStats barracksStats;

    void IUnitSpawner.ProduceUnit(GameObject unitToSpawn)
    {
        throw new System.NotImplementedException();
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

    private void Start()
    {
        Initialize(barracksStats);
        GetComponent<SpriteRenderer>().sortingOrder = 1;
    }

    
}