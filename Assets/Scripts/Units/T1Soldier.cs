using UnityEngine;

public class T1Soldier : UnitBase, IClickable, IDamageable, IMovable, IDamageDealer
{
    [SerializeField] private UnitStats T1SoldierStats;

    private void Start()
    {
        Initialize(T1SoldierStats);
        
    }
}