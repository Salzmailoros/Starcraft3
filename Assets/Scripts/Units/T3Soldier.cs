using UnityEngine;

public class T3Soldier : UnitBase
{
    [SerializeField] private UnitStats T3SoldierStats;

    private void Start()
    {
        Initialize(T3SoldierStats);
    }
}