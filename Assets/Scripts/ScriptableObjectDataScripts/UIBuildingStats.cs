using UnityEngine;

[CreateAssetMenu(fileName = "NewUIBuildingStats", menuName = "GameUI/UIBuildingStats")]
public class UIBuildingStats : ScriptableObject
{
    public BuildingStats buildingStats; // Reference to the main building data
    public Sprite buttonIcon; // Icon to be displayed on the button
    public string buttonText; // Text to be displayed on the button
}
