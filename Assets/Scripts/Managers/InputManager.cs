using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private BuildingPlacementManager buildingPlacer;


    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (buildingPlacer.IsPlacing()) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButtonDown(0))    //leftclick basic
        {
            SelectionManager.Instance.ClearSelection();
            RaycastToClickable(true);
        }
        else if (Input.GetMouseButtonDown(1))   //rightclick basic.
        {
            RaycastToClickable(false);
        }
    }

    private void RaycastToClickable( bool isLeftClick)
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider!=null)
        {
            IClickable clickable = hit.collider.GetComponent<IClickable>();
            if (clickable != null)
            {
                if (isLeftClick)        //leftclick on clickable unit logic
                {
                    clickable.OnLeftClick();
                    SelectionManager.Instance.SelectUnit(clickable);
                }
                else                  //rigtclick on clickable unit logic
                {
                    clickable.OnRightClick();
                }
            }
        }
    }
}
