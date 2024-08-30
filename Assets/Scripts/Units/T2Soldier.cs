using UnityEngine;

public class T2Soldier : UnitBase
{
    [SerializeField] private UnitStats T2SoldierStats;

    private void Start()
    {
        Initialize(T2SoldierStats);
    }
}