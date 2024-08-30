using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }
    public event Action<IClickable> OnSelectionChanged;

    private IClickable selectedObject;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

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
