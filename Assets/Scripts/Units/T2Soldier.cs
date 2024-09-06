using UnityEngine;

public class T2Soldier : UnitBase, IClickable, IDamageable, IDamageDealer
{
    [SerializeField] private UnitStats T2SoldierStats;

    public void OnLeftClick()
    {
        SelectionManager.Instance.SelectUnit(this);
    }

    public void OnRightClick()
    {
        Debug.Log("TARGETED ME :" + name);
    }

    private void Start()
    {
        Initialize(T2SoldierStats);
    }
    public override void Die()
    {
        base.Die();
        Debug.Log($" {this.name} has been destroyed.");
    }
}