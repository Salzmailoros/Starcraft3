using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitStats", menuName = "Game/UnitStats")]
public class UnitStats : ScriptableObject
{
    public string unitName;
    public int health;
    public int damage;
    public float movementSpeed;
    public Vector2Int size;  
    public Sprite unitSprite;
    public Sprite uiSprite;



    public GameObject unitPrefab;
}