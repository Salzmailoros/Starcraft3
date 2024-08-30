using UnityEngine;

public class PowerPlant : BuildingBase
{
    [SerializeField] private BuildingStats PowerPlantStats;

    private void Start()
    {
        Initialize(PowerPlantStats);
        GetComponent<SpriteRenderer>().sortingOrder = 1;
    }
}