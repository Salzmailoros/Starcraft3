using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BuildingEvent : UnityEvent<BuildingStats> { }

public static class BuildingEventSystem
{
    // Event triggered when a building placement is requested
    public static BuildingEvent OnBuildingPlacementRequested = new BuildingEvent();

    // This method can be called to request a building placement
    public static void RequestBuildingPlacement(BuildingStats buildingStats)
    {
        OnBuildingPlacementRequested.Invoke(buildingStats);
    }
}
