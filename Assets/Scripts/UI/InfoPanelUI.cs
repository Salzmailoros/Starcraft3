using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InfoPanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text unitInfoText;
    [SerializeField] private TMP_Text unitHPText;
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
            handleSpawnableContent(null);
        }
        else if (currentSelectedObject is BuildingBase building)
        {
            BuildingStats stats = building.ReturnInfoPanelInfo();
            unitInfoText.text = stats.name;
            unitImage.sprite = stats.uiSprite;
            unitMaxHP = stats.health;
            if (building.GetComponent<IObjectSpawner>()!= null)         // if building has Iobjectspawner on
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
            unitInfoText.text = "No selection";
            unitImage.sprite = null;
            unitHPText.text = " ";
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
                Instantiate(stats.ProduceableUnits[i].GetComponent<UnitStats>().uiSprite,
                    Vector3.zero, Quaternion.identity, contentForInfoPanel.transform);
                
            }
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
