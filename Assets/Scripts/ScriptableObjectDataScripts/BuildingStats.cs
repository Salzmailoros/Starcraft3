
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingStats", menuName = "Game/BuildingStats")]
public class BuildingStats : ScriptableObject
{
    public string buildingName;
    public int health;
    public Vector2Int size;
    public Sprite buildingSprite;
    public Sprite damagedSprite;
    public Sprite deadSprite;
    public Sprite uiSprite;

    public GameObject buildingPrefab;
}