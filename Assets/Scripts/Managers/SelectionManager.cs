using System;

public class SelectionManager : Singleton<SelectionManager>
{
    public event Action<IClickable> OnSelectionChanged; //only used inside buttons. could be replaced to work with
                                                        // proper singleton usage but kept here as an example for 
                                                        // event system usage. called in UIBuildingButton.
                                                        // UIUnitSpawnButton uses this as a singleton.
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
