using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InfoPanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text selectionInfoText;
    [SerializeField] private TMP_Text selectionHPText;
    [SerializeField] private Image unitImage;
    [SerializeField] private GameObject contentForInfoPanel;

    private int unitMaxHP;

    private IClickable currentSelectedObject;

    private void OnEnable()
    {
        SelectionManager.Instance.OnSelectionChanged += OnSelectionChanged;
    }

    private void OnDisable()
    {
        SelectionManager.Instance.OnSelectionChanged -= OnSelectionChanged;
    }

    private void Update()
    {
        if (currentSelectedObject != null)
        {
            UpdateHealth(); 
        }
    }

    private void OnSelectionChanged(IClickable selectedObject)
    {
        currentSelectedObject = selectedObject;
        SelectionManager.Instance.UpdateSelectionto(selectedObject);

        // Check for null and update UI accordingly
        if (currentSelectedObject != null)
        {
            UpdateStaticUI();
        }
        else
        {
            selectionInfoText.text = "No selection";
            unitImage.sprite = null;
            selectionHPText.text = " ";
            handleSpawnableContent(null);
        }
    }


    private void UpdateStaticUI()
    {
        if (currentSelectedObject is UnitBase unit)
        {
            UnitStats stats = unit.ReturnInfoPanelInfo();
            selectionInfoText.text = stats.name;
            unitImage.sprite = stats.uiSprite;
            unitMaxHP = stats.health ;
            handleSpawnableContent(null);
        }
        else if (currentSelectedObject is BuildingBase building)
        {
            BuildingStats stats = building.BuildingStats();
            selectionInfoText.text = stats.name;
            unitImage.sprite = stats.uiSprite;
            unitMaxHP = stats.health;
            if (building.GetComponent<IUnitSpawner>()!= null)         // if building has Iobjectspawner on
            {
                handleSpawnableContent(stats);
            }
            else
            {
                handleSpawnableContent(null);                           //if building is not objectspawner
            }
        }
        else
        {
            selectionInfoText.text = "No selection";
            unitImage.sprite = null;
            selectionHPText.text = " ";
            handleSpawnableContent(null);
        }


    }

    private void handleSpawnableContent(BuildingStats stats)
    {
        if (stats == null)
        {
            ClearContentForInfoPanel();
            return;
        }

        // Safeguard before accessing ProduceableUnits
        if (stats.ProduceableUnits != null && stats.ProduceableUnits.Length > 0)
        {
            foreach (var unitStats in stats.ProduceableUnits)
            {
                GameObject newButton = CreateUnitButton(unitStats);
                newButton.transform.SetParent(contentForInfoPanel.transform, false);
            }
        }
    }

    // Clear the content panel
    private void ClearContentForInfoPanel()
    {
        foreach (Transform child in contentForInfoPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Refactor button creation for reuse
    private GameObject CreateUnitButton(UnitStats unitStats)
    {
        GameObject newButton = new GameObject("UI Button");
        Button buttonComponent = newButton.AddComponent<Button>();
        UIUnitSpawnButton uiUnitSpawnButton = newButton.AddComponent<UIUnitSpawnButton>();
        uiUnitSpawnButton.SetUnitStats(unitStats);

        Image imageComponent = newButton.AddComponent<Image>();
        imageComponent.sprite = unitStats.uiSprite;

        return newButton;
    }


    private void UpdateHealth()
    {
        if (currentSelectedObject is UnitBase unit)
        {
            selectionHPText.text = "Hp :" + unit.currentHealth.ToString() + " / " + unitMaxHP;
        }
        else if (currentSelectedObject is BuildingBase building)
        {
            selectionHPText.text = "Hp :" + building.currentHealth.ToString() + " / " + unitMaxHP;
        }
        else
        {
            selectionHPText.text = " ";
        }
    }
}
