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
        SelectionManager.Instance.SelectUnit(selectedObject);
        UpdateStaticUI();
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
            BuildingStats stats = building.ReturnInfoPanelInfo();
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
        if (stats == null) return;
        foreach (GameObject item in contentForInfoPanel.transform)
        {
            Destroy(item);
        }
        if (stats.ProduceableUnits != null)
        {
            for (int i = 0; i < stats.ProduceableUnits.Length; i++)
            {
                Debug.Log(stats.ProduceableUnits[i].name);
                GameObject newButton = new GameObject("UI Button");
                newButton.AddComponent<Button>();
                newButton.AddComponent<UIUnitSpawnButton>();
                Image imageComponent = newButton.AddComponent<Image>();
                imageComponent.sprite = stats.ProduceableUnits[i].uiSprite;

                newButton.transform.SetParent(contentForInfoPanel.transform, false);

            }
        }
        
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
