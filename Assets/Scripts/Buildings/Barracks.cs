using UnityEngine;

public class Barracks : BuildingBase
{
    [SerializeField] private BuildingStats barracksStats;

    private void Start()
    {
        Initialize(barracksStats);
        GetComponent<SpriteRenderer>().sortingOrder = 1;
    }
}