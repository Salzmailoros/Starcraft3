using UnityEngine;

public class PowerPlant : BuildingBase, IClickable, IDamageable
{
    [SerializeField] private BuildingStats PowerPlantStats;
    void IDamageable.Die()
    {
        base.Die();
    }

    void IClickable.OnLeftClick()
    {
        Debug.Log($" Selected Building of type : {buildingStats.buildingName}");
        // update the UI to show this buildings menu/stats/stuff
    }

    void IClickable.OnRightClick()
    {
        Debug.Log($"Targeted Building : {buildingStats.buildingName}");
    }

    private void Awake()
    {
        Initialize(PowerPlantStats);
        GetComponent<SpriteRenderer>().sortingOrder = 1;
    }

} 