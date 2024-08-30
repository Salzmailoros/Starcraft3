using UnityEngine;
using UnityEngine.Events;

public class UIPlacementController : MonoBehaviour
{
    public BuildingEvent OnBuildingSelected = new BuildingEvent();

    public void SelectBuilding(BuildingStats buildingStats)
    {
        // Trigger the event to place the building
        OnBuildingSelected.Invoke(buildingStats);
    }

    private void OnEnable()
    {
        OnBuildingSelected.AddListener(HandleBuildingPlacement);
    }

    private void OnDisable()
    {
        OnBuildingSelected.RemoveListener(HandleBuildingPlacement);
    }

    private void HandleBuildingPlacement(BuildingStats buildingStats)
    {
        // Logic to instantiate ghost model at mouse position
        // Check if the placement is valid
        // If valid, place the building; otherwise, show feedback (like red color)
        Debug.Log($"Selected building: {buildingStats.buildingName}");
        // Further implementation for ghost model and placement logic goes here
    }
}
