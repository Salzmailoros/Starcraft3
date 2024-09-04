using UnityEngine;

public class T3Soldier : UnitBase, IClickable, IDamageable, IMovable, IDamageDealer
{
    [SerializeField] private UnitStats T3SoldierStats;

    private void Start()
    {
        Initialize(T3SoldierStats);
    }
}