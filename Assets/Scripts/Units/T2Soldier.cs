using UnityEngine;

public class T2Soldier : UnitBase, IClickable, IDamageable, IMovable, IDamageDealer
{
    [SerializeField] private UnitStats T2SoldierStats;

    private void Start()
    {
        Initialize(T2SoldierStats);
    }
}