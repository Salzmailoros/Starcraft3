using UnityEngine;
using UnityEngine.UI;

public class UIUnitSpawnButton : MonoBehaviour
{
    [SerializeField] private UnitStats unitStats;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }
    public void SetUnitStats(UnitStats statsToSet)
    {
        unitStats=statsToSet;
    }

    private void OnClick()
    {

        BuildingBase currentBuilding = SelectionManager.Instance.GetSelectedObject() as BuildingBase;
        if (currentBuilding == null) { Debug.LogWarning("non building trying to spawn a unit"); return; };

        SpawnManager.Instance.SpawnSoldierFromPool(unitStats.name, currentBuilding.gridPos, currentBuilding.BuildingStats().size);
        
    }

}
