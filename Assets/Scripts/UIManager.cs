using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager GetUIManager()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType<UIManager>();
            if (instance == null)
            {
                GameObject container = new GameObject("UI Manager");
                instance = container.AddComponent<UIManager>();
            }
        }
        return instance;
    }

    private static UIManager instance;

    [SerializeField]
    private GameObject shopUI;

    // UI object
    [SerializeField]
    private Button upgradeButton;
    public Button UpgradeButton { get { return upgradeButton; } }
    [SerializeField]
    private Button exitButton;
    public Button ExitButton { get { return exitButton; } }
    [SerializeField]
    private Button informationButton;
    public Button InformationButton { get { return informationButton; } }
    [SerializeField]
    private Button unitSelectButton;
    public Button UnitSelectButton { get { return unitSelectButton; } }
    [SerializeField]
    private TMP_Text levelText;
    public TMP_Text LevelText { get { return levelText; } }

 
    void Start()
    {

    }

    void Update()
    {
        if (!GameManager.GetGameManager().isBuildingUIOn) HideBuildingPopupUI();
    }

    public void ShowBuildingPopupUI(ref BuildingData buildingData)
    {
        UpdateBuildingDataUI(ref buildingData);
        informationButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        exitButton.onClick.AddListener(this.HideBuildingPopupUI);
        if (buildingData.type == "main")
            return;
        levelText.gameObject.SetActive(true);
        upgradeButton.gameObject.SetActive(true);
        if (GameManager.GetGameManager().PlayerData.money < buildingData.upgradeCost || GameManager.GetGameManager().BuildingLimitLevel <= buildingData.currentLevel)
        {
            Color color = upgradeButton.image.color;
            color = Color.red;
            upgradeButton.image.color = color;
        }
        unitSelectButton.gameObject.SetActive(true);
    }

    public void HideBuildingPopupUI()
    {
        upgradeButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
        levelText.gameObject.SetActive(false);
        upgradeButton.gameObject.SetActive(false);
        informationButton.gameObject.SetActive(false);
        unitSelectButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        GameManager.GetGameManager().IsSelected = false;
        Color color = upgradeButton.image.color;
        color = Color.white;
        upgradeButton.image.color = color;
    }

    public void UpdateBuildingDataUI(ref BuildingData buildingData)
    {
        UIManager.GetUIManager().LevelText.text = "building : " + buildingData.currentLevel.ToString();
    }

    public void ShowShopUI()
    {
        shopUI.SetActive(true);
    }

    public void HideShopUI()
    {
        shopUI.SetActive(false);
    }
}
