using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacementManager : MonoBehaviour
{
    [SerializeField] private GridGenerator gridGenerator;
    private BuildingStats currentBuildingStats;
    private GameObject currentBuildingInstance;
    private bool isPlacingBuilding = false;

    [SerializeField] private GameObject placeable, notPlaceable;

    private Vector2Int lastcalcedpos;

    private void Start()
    {
        lastcalcedpos = new Vector2Int(500, 500);
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

        foreach (Transform child in transform)          //destroy highlighted blocks
        {
            Destroy(child.gameObject);
        }

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
        Vector2Int calcedCornerGridAdress = gridGenerator.WorldPositionToGrid(corner);

        if (lastcalcedpos == calcedCornerGridAdress)
        {
            return;
        }
        lastcalcedpos = calcedCornerGridAdress;

        // Clear existing highlighted tiles
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                var tilepos = new Vector2Int(calcedCornerGridAdress.x + x, calcedCornerGridAdress.y + y);
                if (gridGenerator.IsValidGridPosition(tilepos))
                {
                    Instantiate(placeable,
                        new Vector3(gridGenerator.GridToWorldPosition(new Vector2Int(calcedCornerGridAdress.x + x, 0)).x,
                        gridGenerator.GridToWorldPosition(new Vector2Int(0, calcedCornerGridAdress.y + y)).y, 0),
                        Quaternion.identity, transform);
                }
                else
                {
                    Instantiate(notPlaceable,
                        new Vector3(gridGenerator.GridToWorldPosition(new Vector2Int(calcedCornerGridAdress.x + x, 0)).x,
                        gridGenerator.GridToWorldPosition(new Vector2Int(0, calcedCornerGridAdress.y + y)).y, 0),
                        Quaternion.identity, transform);
                }
            }
        }
    }

    private Vector2 FindBuildingBottomLeftCorner()
    {
        var bottomleftBlockCenterCoordinates = new Vector2(
        currentBuildingInstance.transform.position.x - (currentBuildingStats.size.x / 2 * 0.32f) + 0.3f,
        currentBuildingInstance.transform.position.y - (currentBuildingStats.size.y / 2 * 0.32f) + 0.3f
        );
        return bottomleftBlockCenterCoordinates;
    }

    private void TryPlaceBuilding()
    {
        Vector2Int calcedCornerGridAdress = gridGenerator.WorldPositionToGrid(FindBuildingBottomLeftCorner());

        // Check if all tiles are available
        for (int x = 0; x < currentBuildingStats.size.x; x++)
        {
            for (int y = 0; y < currentBuildingStats.size.y; y++)
            {
                var tilepos = new Vector2Int(calcedCornerGridAdress.x + x, calcedCornerGridAdress.y + y);
                Tile tile = gridGenerator.GetTile(tilepos);

                if (tile == null || tile.IsOccupied)
                {
                    Debug.Log("Cannot place building. Tile is occupied or out of bounds.");
                    return; // Exit if any tile is occupied or invalid
                }
            }
        }

        // If all tiles are available, set them as occupied
        for (int x = 0; x < currentBuildingStats.size.x; x++)
        {
            for (int y = 0; y < currentBuildingStats.size.y; y++)
            {
                var tilepos = new Vector2Int(calcedCornerGridAdress.x + x, calcedCornerGridAdress.y + y);
                Tile tile = gridGenerator.GetTile(tilepos);

                if (tile != null)
                {
                    tile.SetOccupied(currentBuildingInstance);
                }
            }
        }

        // Calculate the correct center position for the building
        Vector2 buildingCenter = gridGenerator.GridToWorldPosition(calcedCornerGridAdress);
        float tileSize = gridGenerator.GetTileSize();

        buildingCenter.x += (currentBuildingStats.size.x - 1) * tileSize / 2;
        buildingCenter.y += (currentBuildingStats.size.y - 1) * tileSize / 2;
        currentBuildingInstance.transform.position = buildingCenter;

        // Clear highlighted tiles
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Stop placing the building
        isPlacingBuilding = false;
        currentBuildingInstance = null;
    }
    public bool isPlacing()
    {
        return isPlacingBuilding;
    }

}
