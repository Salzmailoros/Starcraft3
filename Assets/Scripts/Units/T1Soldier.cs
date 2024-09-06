using UnityEngine;

public class T1Soldier : UnitBase, IClickable, IDamageable, IDamageDealer
{
    [SerializeField] private UnitStats T1SoldierStats;

    private void Start()
    {
        Initialize(T1SoldierStats);
    }

    public void OnLeftClick()
    {
        SelectionManager.Instance.SelectUnit(this);
    }

    public void OnRightClick()
    {
        Debug.Log("TARGETED ME :" + name);
    }

    public override void Die()
    {
        base.Die();
        Debug.Log($" {this.name} has been destroyed.");
    }
}
