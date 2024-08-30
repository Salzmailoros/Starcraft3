using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text unitInfoText;
    [SerializeField] private TMP_Text unitHPText;
    [SerializeField] private Image unitImage;

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
        UpdateStaticUI();
    }

    private void UpdateStaticUI()
    {
        if (currentSelectedObject is UnitBase unit)
        {
            UnitStats stats = unit.ReturnInfoPanelInfo();
            unitInfoText.text = stats.name;
            unitImage.sprite = stats.uiSprite;
            unitMaxHP = stats.health ;
        }
        else if (currentSelectedObject is BuildingBase building)
        {
            BuildingStats stats = building.ReturnInfoPanelInfo();
            unitInfoText.text = stats.name;
            unitImage.sprite = stats.uiSprite;
            unitMaxHP = stats.health;
        }
        else
        {
            unitInfoText.text = "No selection";
            unitImage.sprite = null;
            unitHPText.text = " ";
        }
    }

    private void UpdateHealth()
    {
        if (currentSelectedObject is UnitBase unit)
        {
            unitHPText.text = "Hp :" + unit.currentHealth.ToString() + " / " + unitMaxHP;
        }
        else if (currentSelectedObject is BuildingBase building)
        {
            unitHPText.text = "Hp :" + building.currentHealth.ToString() + " / " + unitMaxHP;
        }
        else
        {
            unitHPText.text = " ";
        }
    }
}
