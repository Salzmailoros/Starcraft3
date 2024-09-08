using System.Collections.Generic;
using UnityEngine;

public class LevelPlacer : MonoBehaviour
{
    [SerializeField] private BuildingStats powerGeneratorStats;  
    [SerializeField] private BuildingStats barracksStats;        
    [SerializeField] private UnitStats soldierStats;            
    [SerializeField] private List<Vector2Int> powerGeneratorPositions; 
    [SerializeField] private Vector2Int barracksPosition;        
    [SerializeField] private List<Vector2Int> soldierPositions; 
    [SerializeField] private Transform buildingParent;  
    [SerializeField] private Transform unitParent;      

    public void SpawnStuff()
    {
        // Place buildings
        PlaceBuilding(powerGeneratorStats, powerGeneratorPositions[0]);
        PlaceBuilding(barracksStats, barracksPosition);
        
        // Place soldiers
        PlaceSoldiers();

        foreach (Transform child in transform) 
        {
            var childID = child.GetComponent<IDamageable>(); 
            if (childID != null)  
            {
                childID.TeamID = 1;  
            }
        }

    }

    // Method to place buildings while checking grid size availability
    private void PlaceBuilding(BuildingStats buildingStats, Vector2Int bottomLeftPosition)
    {
        if (buildingStats == null || buildingStats.buildingPrefab == null)
        {
            Debug.LogError("Building stats or prefab is null.");
            return;
        }

        Vector2Int buildingSize = buildingStats.size;

        if (CanPlaceBuilding(bottomLeftPosition, buildingSize))
        {
            Vector3 worldPosition = GridManager.Instance.GridToWorldPosition(bottomLeftPosition);
            worldPosition = worldPosition + new Vector3(buildingSize.x*0.16f,buildingSize.y*0.16f, 0) - new Vector3(0.16f,0.16f,0);
            // Instantiate building prefab from BuildingStats
            GameObject newBuilding = Instantiate(buildingStats.buildingPrefab, worldPosition, Quaternion.identity, buildingParent);

            // Set grid position in BuildingBase (assuming the building prefab has this component)
            BuildingBase newBuildingBase = newBuilding.GetComponent<BuildingBase>();
            if (newBuildingBase != null)
            {
                newBuildingBase.gridPos = bottomLeftPosition;
            }

            // Mark the grid tiles as occupied
            SetBuildingTiles(bottomLeftPosition, buildingSize, newBuilding);

            if (buildingStats is IUnitSpawner)
            {
                IUnitSpawner spawner = buildingStats as IUnitSpawner;
                if (spawner != null)  // If the object is a unit spawner create pool if not there already.
                {
                    spawner.CheckInitialisePool();
                }
            }

            
        }
        else
        {
            Debug.LogWarning($"Cannot place building at {bottomLeftPosition}. Not enough space.");
        }
    }

    private bool CanPlaceBuilding(Vector2Int bottomLeftPosition, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int checkPos = new Vector2Int(bottomLeftPosition.x + x, bottomLeftPosition.y + y);
                if (!GridManager.Instance.IsValidGridPosition(checkPos))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void SetBuildingTiles(Vector2Int bottomLeftPosition, Vector2Int size, GameObject building)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int setPos = new Vector2Int(bottomLeftPosition.x + x, bottomLeftPosition.y + y);
                GridManager.Instance.SetTile(setPos, building);
            }
        }
    }

    private void PlaceSoldiers()
    {
        foreach (Vector2Int soldierPosition in soldierPositions)
        {
            if (GridManager.Instance.IsValidGridPosition(soldierPosition))
            {
                GameObject newSoldier = Instantiate(soldierStats.unitPrefab, unitParent);
                if (newSoldier == null)
                {
                    Debug.LogError("Failed to instantiate soldier.");
                    continue;
                }

                // Set the soldier's position to the corresponding grid position
                Vector3 worldPosition = GridManager.Instance.GridToWorldPosition(soldierPosition);
                newSoldier.transform.position = worldPosition;

                // Mark the tile as occupied by this soldier
                GridManager.Instance.SetTile(soldierPosition, newSoldier);

            }
            else
            {
                //Grid occupied cant spawn.
            }
        }
    }
}
