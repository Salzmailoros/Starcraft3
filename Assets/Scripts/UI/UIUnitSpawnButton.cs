using UnityEngine;
using UnityEngine.UI;

public class UIUnitSpawnButton : MonoBehaviour
{
    [SerializeField] private UnitStats unitStats;
    private Button button;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        //something something building.Iunitspawner.spawn => spawnmanager.instance.spawnfrompool.
       //SpawnManager.Instance.SpawnFromPool(unitStats.name,SelectionManager.Instance.GetSelectedObject().transform.position,Quaternion.identity);
    }
    
}
