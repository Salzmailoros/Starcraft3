using UnityEngine;

public class T1Soldier : UnitBase, IClickable, IDamageable, IDamageDealer
{
    [SerializeField] private UnitStats T1SoldierStats;

    public int TeamID = 0;

    int IDamageable.TeamID { get => TeamID; set => TeamID = value; }
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

}
