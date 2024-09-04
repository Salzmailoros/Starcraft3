using UnityEngine;
using UnityEngine.UI;

public class UIBuildingButton : MonoBehaviour
{
    [SerializeField] private BuildingStats buildingStats; // The building data associated with this button
    private Button button;
    private Image buttonImage;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        // Set sprite incase I forget to update it after creating a new building
        buttonImage.sprite = buildingStats.buildingSprite;

        // Add the OnClick listener
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // Trigger the event to request building placement
        BuildingEventSystem.RequestBuildingPlacement(buildingStats);
    }
}
