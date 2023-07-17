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
    private GameObject buildingUIpref;
    [SerializeField]
    private Transform buildingListUIParent;


    // UI object
    [SerializeField]
    private Button mUpgradeButton;
    [SerializeField]
    public Button UpgradeButton { get { return mUpgradeButton; } }
    [SerializeField]
    private Button mExitButton;
    public Button ExitButton { get { return mExitButton; } }
    [SerializeField]
    private TMP_Text mLevelText;
    public TMP_Text LevelText { get { return mLevelText; } }
    [SerializeField]
    private TMP_Text mIDButton;
    public TMP_Text IDButton { get { return mIDButton; } }
    [SerializeField]
    private GameObject shopUI;
 

    // Start is called before the first frame update
    private void Awake()
    {
    }
    void Start()
    {
        CreateBuildingListShopUI();

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.GetGameManager().isBuildingUIOn) HideBuildingPopupUI();
    }
    public void ShowBuildingPopupUI(ref BuildingData buildingData)
    {
        UpdateBuildingDataUI(ref buildingData);
        if (GameManager.GetGameManager().PlayerData.money < buildingData.upgradeCost || GameManager.GetGameManager().BuildingLimitLevel <= buildingData.currentLevel)
        {
            Color color = mUpgradeButton.image.color;
            color = Color.red;
            mUpgradeButton.image.color = color;
        }
        mLevelText.gameObject.SetActive(true);
        mUpgradeButton.gameObject.SetActive(true);
        mIDButton.gameObject.SetActive(true);
        mExitButton.gameObject.SetActive(true);
        mExitButton.onClick.AddListener(this.HideBuildingPopupUI);
    }

    public void HideBuildingPopupUI()
    {
        mUpgradeButton.onClick.RemoveAllListeners();
        mExitButton.onClick.RemoveAllListeners();
        mLevelText.gameObject.SetActive(false);
        mUpgradeButton.gameObject.SetActive(false);
        mIDButton.gameObject.SetActive(false);
        mExitButton.gameObject.SetActive(false);
        GameManager.GetGameManager().IsSelected = false;
        Color color = mUpgradeButton.image.color;
        color = Color.white;
        mUpgradeButton.image.color = color;
    }

    public void UpdateBuildingDataUI(ref BuildingData buildingData)
    {
        mLevelText.text = "building : " + buildingData.currentLevel.ToString();
        mIDButton.text = buildingData.id;
    }

    private void CreateBuildingListShopUI()
    {
        foreach (BuildingPrice buildingPrice in DataManager.GetDataManager().BuildingPricesList)
        {
            GameObject buildingUI = Instantiate(buildingUIpref, buildingListUIParent);
            buildingUI.GetComponent<BuildingUI>().Setup(buildingPrice);
        }
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
