using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

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
        if (buildingPlacer.IsPlacing()) return;  // If placing a building, don't handle input
        if (EventSystem.current.IsPointerOverGameObject()) return;  // Ignore input over UI elements

        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            SelectionManager.Instance.ClearSelection();
            RaycastToClickable(true);
        }
        else if (Input.GetMouseButtonDown(1)) // Right-click
        {
            RaycastToClickable(false);
        }
    }

    private void RaycastToClickable(bool isLeftClick)
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            IClickable clickable = hit.collider.GetComponent<IClickable>();

            if (clickable != null)
            {
                if (isLeftClick)  // Left-click selects the unit
                {
                    clickable.OnLeftClick();
                    SelectionManager.Instance.SelectUnit(clickable);
                }
                else  // Right-click handles commands (attack/move)
                {
                    HandleRightClick(clickable);
                }
            }
        }
        else  // If clicked on an empty space (for movement)
        {
            HandleEmptyRightClick();
        }
    }

    private void HandleRightClick(IClickable clickable)
    {
        if (SelectionManager.Instance.GetSelectedObject() is UnitBase)
        {
            var currentSelection = SelectionManager.Instance.GetSelectedObject() as UnitBase;

            if (clickable is IDamageable)  // Right-click on a damageable target (attack)
            {
                var target = clickable as IDamageable;

                // Reset current command and attack the new target
                currentSelection.isCommandOverride = true;
                currentSelection.Attack(target);  // Attack command
            }

            clickable.OnRightClick();  // Trigger right-click event on the clickable object
        }
    }

    private void HandleEmptyRightClick()
    {
        // Right-click on empty space - move the unit
        if (SelectionManager.Instance.GetSelectedObject() is UnitBase)
        {
            var currentSelection = SelectionManager.Instance.GetSelectedObject() as UnitBase;
            var targetPos = GridManager.Instance.WorldPositionToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            Debug.Log("Target position: " + targetPos);

            currentSelection.isCommandOverride = true;  // Override current command
            currentSelection.MoveTo(targetPos);  // Issue movement command
        }
    }
}
