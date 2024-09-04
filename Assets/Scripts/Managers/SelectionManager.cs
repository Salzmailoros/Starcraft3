using System;
using UnityEngine;

public class SelectionManager : Singleton<SelectionManager>
{
    public event Action<IClickable> OnSelectionChanged;

    private IClickable selectedObject;

    public void SelectUnit(IClickable unit)
    {
        selectedObject = unit;
        OnSelectionChanged?.Invoke(selectedObject);
    }

    public void DeselectUnit()
    {
        selectedObject = null;
        OnSelectionChanged?.Invoke(null);
    }

    public void ClearSelection()
    {
        selectedObject = null;
        OnSelectionChanged?.Invoke(null);
    }

    public IClickable GetSelectedObject()
    {
        return selectedObject;
    }
}
