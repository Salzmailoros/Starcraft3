using UnityEngine;

public class PowerPlant : BuildingBase, IClickable, IDamageable
{
    [SerializeField] private BuildingStats PowerPlantStats;
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
        Initialize(PowerPlantStats);
        GetComponent<SpriteRenderer>().sortingOrder = 1;
    }

} 