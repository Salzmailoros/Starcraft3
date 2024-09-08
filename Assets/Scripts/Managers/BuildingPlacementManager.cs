using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacementManager : MonoBehaviour
{
    private GridManager gridManager;
    private BuildingStats currentBuildingStats;
    private GameObject currentBuildingInstance;
    private bool isPlacingBuilding = false;

    [SerializeField] private GameObject placeable, notPlaceable;

    private Vector2Int lastcalcedpos;

    private void Start()
    {
        lastcalcedpos = new Vector2Int(500, 500);
        gridManager = GridManager.Instance;
        PoolManager.Instance.CreatePool("placeablePoolKey", placeable, 10); 
        PoolManager.Instance.CreatePool("notPlaceablePoolKey", notPlaceable, 10); 
    }

    private void OnEnable()
    {
        BuildingEventSystem.OnBuildingPlacementRequested.AddListener(StartPlacingBuilding);
    }

    private void OnDisable()
    {
        BuildingEventSystem.OnBuildingPlacementRequested.RemoveListener(StartPlacingBuilding);
    }

    private void Update()
    {
        if (isPlacingBuilding)
        {
            FollowMouse();

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                TryPlaceBuilding();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape)) { StopPlacingBuilding(); }        // press esc = cancel placing.
    }

    public void StartPlacingBuilding(BuildingStats buildingStats)
    {
        currentBuildingStats = buildingStats;
        currentBuildingInstance = Instantiate(currentBuildingStats.buildingPrefab);
        isPlacingBuilding = true;
    }
    public void StopPlacingBuilding()       // cancel placing
    {
        DestroyImmediate(currentBuildingInstance);      //destroy building on mouse

        resetHighlightedObjects();
        currentBuildingStats = null;                    //reset
        currentBuildingInstance = null;
        isPlacingBuilding = false;
    }

    private void FollowMouse()
    {
        currentBuildingInstance.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        HighlightTiles(FindBuildingBottomLeftCorner(), currentBuildingStats.size);
    }

    private void HighlightTiles(Vector2 corner, Vector2 size)
    {
        Vector2Int calcedCornerGridAdress = gridManager.WorldPositionToGrid(corner);

        if (lastcalcedpos == calcedCornerGridAdress)
        {
            return;//leave if still at same coordinates.
        }
        resetHighlightedObjects();

        lastcalcedpos = calcedCornerGridAdress;

        // Clear existing highlighted tiles

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                GameObject highligtObject;
                var tilepos = new Vector2Int(calcedCornerGridAdress.x + x, calcedCornerGridAdress.y + y);
                if (gridManager.IsValidGridPosition(tilepos))
                {
                    highligtObject = SpawnManager.Instance.SpawnHighlightObjects("placeablePoolKey",
                        new Vector3(gridManager.GridToWorldPosition(new Vector2Int(calcedCornerGridAdress.x + x, 0)).x,
                        gridManager.GridToWorldPosition(new Vector2Int(0, calcedCornerGridAdress.y + y)).y));
                    highligtObject.transform.SetParent(transform);
                   
                }
                else
                {
                    highligtObject = SpawnManager.Instance.SpawnHighlightObjects("notPlaceablePoolKey",
                        new Vector3(gridManager.GridToWorldPosition(new Vector2Int(calcedCornerGridAdress.x + x, 0)).x,
                        gridManager.GridToWorldPosition(new Vector2Int(0, calcedCornerGridAdress.y + y)).y));
                    highligtObject.transform.SetParent(transform);

                }
            }
        }
    }

    private Vector2 FindBuildingBottomLeftCorner()
    {
        var bottomleftBlockCenterCoordinates = new Vector2(
        currentBuildingInstance.transform.position.x - (currentBuildingStats.size.x / 2 * 0.32f) + 0.16f,
        currentBuildingInstance.transform.position.y - (currentBuildingStats.size.y / 2 * 0.32f) + 0.16f
        );
        return bottomleftBlockCenterCoordinates;
    }

    private void TryPlaceBuilding()
    {
        Vector2Int calcedCornerGridAdress = gridManager.WorldPositionToGrid(FindBuildingBottomLeftCorner());

        // Check if all tiles are available
        for (int x = 0; x < currentBuildingStats.size.x; x++)
        {
            for (int y = 0; y < currentBuildingStats.size.y; y++)
            {
                var tilepos = new Vector2Int(calcedCornerGridAdress.x + x, calcedCornerGridAdress.y + y);
                Tile tile = gridManager.GetTile(tilepos);

                if (tile == null || tile.IsOccupied)// Exit if any tile is occupied or invalid
                {
                    Debug.Log("Cannot place building. Tile is occupied or out of bounds.");
                    return; 
                }
            }
        }

        // If all tiles are available, set them as occupied
        for (int x = 0; x < currentBuildingStats.size.x; x++)
        {
            for (int y = 0; y < currentBuildingStats.size.y; y++)
            {
                var tilepos = new Vector2Int(calcedCornerGridAdress.x + x, calcedCornerGridAdress.y + y);
                Tile tile = gridManager.GetTile(tilepos);

                if (tile != null)
                {
                    tile.SetOccupied(currentBuildingInstance);
                }
            }
        }

        // Calculate the correct center position for the building
        Vector2 buildingCenter = gridManager.GridToWorldPosition(calcedCornerGridAdress);
        float tileSize = gridManager.GetTileSize();

        buildingCenter.x += (currentBuildingStats.size.x - 1) * tileSize / 2;
        buildingCenter.y += (currentBuildingStats.size.y - 1) * tileSize / 2;
        currentBuildingInstance.transform.position = buildingCenter;
        currentBuildingInstance.GetComponent<BuildingBase>().gridPos = calcedCornerGridAdress;
        // Clear highlighted tiles
        resetHighlightedObjects();
        IUnitSpawner spawner = currentBuildingInstance.GetComponent<IUnitSpawner>();
        if (spawner != null)  // If the object is a unit spawner create pool if not there already.
        {
            spawner.CheckInitialisePool();
        }

        // Stop placing the building
        isPlacingBuilding = false;
        currentBuildingInstance = null;
    }
    public bool IsPlacing()
    {
        return isPlacingBuilding;
    }
    private void resetHighlightedObjects()
    {
        List<Transform> children = new List<Transform>();       //making list otherwise objects dont get returned properly.

        foreach (Transform child in transform)
        {
            children.Add(child);
        }

        foreach (Transform child in children)
        {
            SpawnManager.Instance.ReturnToPool(child.GetComponent<PoolableObject>().ReturnKey(), child.gameObject);
        }


    }


}