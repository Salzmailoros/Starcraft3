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
        BuildingBase currentBuilding = SelectionManager.Instance.GetSelectedObject() as BuildingBase;
        if (currentBuilding == null) { Debug.LogWarning("non building trying to spawn a unit"); return; };

        SpawnManager.Instance.SpawnFromPool(unitStats.name, currentBuilding.gridPos, currentBuilding.BuildingStats().size,true);
        //currentBuilding.gridPos;
        // try to spawn at currentbuilding.gridpos converted by gridmanager. > also need to pass on buildingsize
        // to make sure we do that thing where u spawn objects around the building. of which the logic should be dealt with by the spawnmanager with a new class.

        //something something building.Iunitspawner.spawn => spawnmanager.instance.spawnfrompool.
        //SpawnManager.Instance.SpawnFromPool(unitStats.name,SelectionManager.Instance.GetSelectedObject().transform.position,Quaternion.identity);
    }

}
