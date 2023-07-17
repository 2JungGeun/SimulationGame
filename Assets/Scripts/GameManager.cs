using JetBrains.Annotations;
//using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class PlayerData
{
    public int days = 1;
    public int time = 0;
    public int money = 6000;
    public int buildingNum = 0;
}

public class MyBuildingData
{
    public List<BuildingData> buildings;
    public MyBuildingData()
    {
        buildings = new List<BuildingData>();
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager GetGameManager()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType<GameManager>();
            if (instance == null)
            {
                GameObject container = new GameObject("Game Manager");
                instance = container.AddComponent<GameManager>();
            }
        }
        return instance;
    }
    // Start is called before the first frame update
    private static GameManager instance;
    private PlayerData playerData = new PlayerData();
    public PlayerData PlayerData { get { return playerData; } set { playerData = value; } }
    private MyBuildingData myBuildingsData = new MyBuildingData();
    public MyBuildingData MyBuildingsData { get { return myBuildingsData; } set { myBuildingsData = value; } }
    RaycastHit2D mHit;
    private bool mIsSelected = false;
    public bool IsSelected { get { return mIsSelected; } set { mIsSelected = value; } }
    private int mMaxTime = 2000;
    private int mBuildingLimitLevel;
    public int BuildingLimitLevel { get { return mBuildingLimitLevel; } }
    private int mLevelUpMoney = 1000;
    public GameObject buildingPrefeb;
    [SerializeField]
    private TMP_Text daysText;
    [SerializeField]
    private TMP_Text timeText;
    [SerializeField]
    private TMP_Text moneyText;
    public bool isBuildingUIOn = false;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        DataManager.GetDataManager().LoadPlayerDataFromJson();
        DataManager.GetDataManager().LoadBuildingDataFromJson();
        FirstMapOn();
        GridBuildingSystem.gridSystemScript.LoadMap();
        ChangeBuildingLimitLevel();
        UpdateUIText();
        // 게임 시작 시 이전에 보유한 빌딩 생성
        createMyBuildingObject();
    }

    // Update is called once per frame
    void Update()
    {
        MouseClickDown();
    }

    private void createMyBuildingObject()
    {
        foreach (BuildingData building in myBuildingsData.buildings)
        {
            GameObject obj = Instantiate(buildingPrefeb, building.position, Quaternion.identity);
            if (building.type == "defense")
            {
                obj.AddComponent<DefenseBuilding>().BuildingData = building;

            }
            else if (building.type == "production")
            {
                obj.AddComponent<ProductionBuilding>().BuildingData = building;
            }
        }
    }

    private void MouseClickDown() //GameObject 클릭 확인용 building이외에 GameObject 클릭으로 확인 필요없으면 코드 수정할 예정...
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (isBuildingUIOn)
            {
                isBuildingUIOn = false;
            }
            else
            {
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mHit = Physics2D.Raycast(worldPoint, Vector2.zero, 10.0f);
                FindClickedObject();
            }
        }
    }

    private void FindClickedObject()
    {
        if (mHit.collider == null) return;
        Debug.Log(mHit.collider.name);
        if (mHit.collider.CompareTag("Building"))
        {
            //mIsSelected = true;
            mHit.collider.GetComponent<Building>().ShowPopupUI();
        }
    }

    private void ChangeBuildingLimitLevel()
    {
        mBuildingLimitLevel = 4 + (int)System.Math.Truncate((double)(playerData.days / 5));
    }

    private void ChangeMaxTime()
    {
        mMaxTime = 2000 + (playerData.days - 1) * 2000;
    }

    private void AddTime(int usedCost)
    {
        playerData.time += usedCost;
        if (playerData.time > mMaxTime)
        {
            playerData.time = playerData.time - mMaxTime;
            UpdateDays();
            ChangeMaxTime();
            ChangeBuildingLimitLevel();
        }
        UpdateUIText();
    }

    private void UpdateDays()
    {
        playerData.days++;
        //AddMoney(mLevelUpMoney);
        UpdateLevelUpMoney();
    }

    private void UpdateLevelUpMoney()
    {
        mLevelUpMoney = mLevelUpMoney * 15 / 10;
    }

    private void UpdateUIText()
    {
        int time = (int)System.Math.Truncate(((double)playerData.time / (double)mMaxTime) * 24);
        daysText.text = "Days : " + playerData.days.ToString();
        timeText.text = "Time : " + time.ToString();
        moneyText.text = "Money : " + playerData.money.ToString();
    }

    public void AddMoney(int money) { playerData.money += money; }

    public void UseMoney(int money)
    {
        playerData.money -= money;
        AddTime(money);
    }

    public void BuyBuildingInStore(string name, int price)
    {
        UseMoney(price);
        GameObject obj = Instantiate(buildingPrefeb, new Vector3(0, 0, 0), Quaternion.identity);
        BuildingData data = DataManager.GetDataManager().BuildingDataDic[name].DeepCopy();
        if (data.type == "defense")
        {
            obj.AddComponent<DefenseBuilding>().BuildingData = data;
            obj.GetComponent<DefenseBuilding>().BuildingData.id = PlayerData.buildingNum.ToString();
        }
        else if (data.type == "production")
        {
            obj.AddComponent<ProductionBuilding>().BuildingData = data;
            obj.GetComponent<ProductionBuilding>().BuildingData.id = PlayerData.buildingNum.ToString();
        }
        GridBuildingSystem.gridSystemScript.InitializeBuliding(obj.GetComponent<Building>());
        data.position = obj.transform.position;
        MyBuildingsData.buildings.Add(obj.GetComponent<Building>().BuildingData);
        PlayerData.buildingNum++;
    }

    public void click()
    {
        foreach (BuildingData s in myBuildingsData.buildings)
        {
            Debug.Log(s.id + " " + s.position);
        }
    }

    void FirstMapOn() // 아무것도 없는 maintilemap을 저장해주는 함수
    {
        if (PlayerData.buildingNum == 0)  GridBuildingSystem.gridSystemScript.SaveTileMap();
    }
}
