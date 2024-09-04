using UnityEngine;

public class Barracks : BuildingBase, IClickable, IDamageable, IObjectSpawner
{
    [SerializeField] private BuildingStats barracksStats;

    void IObjectSpawner.produceUnit()
    {
        throw new System.NotImplementedException();
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