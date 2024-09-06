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
                    if (SelectionManager.Instance.GetSelectedObject() is UnitBase)
                    {
                        var currentSelection = SelectionManager.Instance.GetSelectedObject() as UnitBase;

                        if (clickable is IDamageable)     // if selected is a damage dealer and right clicked on a damageable target.
                        {
                            var target = clickable as IDamageable;
                            currentSelection.DealDamage(target);
                        }
                        
                                                      
                       
                    }
                    clickable.OnRightClick();
                }
            }
        }
        if (SelectionManager.Instance.GetSelectedObject() as UnitBase)                          // if selected is a unit and right clicked on is not a damageable target.
        {
            var currentSelection = SelectionManager.Instance.GetSelectedObject() as UnitBase;
            var targetpos = GridManager.Instance.WorldPositionToGrid(hit.point);
            currentSelection.MoveTo(targetpos);
        }
    }
}
