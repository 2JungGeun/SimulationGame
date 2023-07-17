using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
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
using UnityEngine.UIElements;

[System.Serializable]
public class PlayerData
{
    public int days;
    public int time;
    public int money;
    public int buildingNum;
    public int unitNum;
    public PlayerData()
    {
        this.days = 1;
        this.time = 0;
        this.money = 6000;
        this.buildingNum = 0;
        this.unitNum = 0;
    }
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

    private MyBuilding myBuildings = new MyBuilding();
    public MyBuilding MyBuildings { get { return myBuildings; } set { myBuildings = value; } }

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
        //DataManager.GetDataManager().LoadBuildingDataFromJson();
        DataManager.GetDataManager().LoadBuildingDataFromBinaryfile();
        CreateMyBuildingObject();
        ChangeBuildingLimitLevel();
        UpdateUIText();
        FirstMapOn();
        GridBuildingSystem.gridSystemScript.LoadMap();
        // ���� ���� �� ������ ������ ���� ����
    }

    // Update is called once per frame
    void Update()
    {
        MouseClickDown();
    }

    private void CreateMyBuildingObject()
    {
        foreach (Building building in myBuildings.buildings)
        {
            GameObject obj = Instantiate(buildingPrefeb, building.BuildingData.position, Quaternion.identity);
            obj.AddComponent<BuildingSystem>().Building = building;
            Debug.Log("실행됨");
        }
    }

    private void MouseClickDown() //GameObject Ŭ�� Ȯ�ο� building�̿ܿ� GameObject Ŭ������ Ȯ�� �ʿ������ �ڵ� ������ ����...
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
                FindClickedObject();
            }
        }
    }

    private void FindClickedObject()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mHit = Physics2D.Raycast(worldPoint, Vector2.zero, 10.0f);
        if (mHit.collider == null) return;
        if (mHit.collider.CompareTag("Building") && mHit.collider.GetComponent<BuildingSystem>().Building.BuildingData.isPlaced == true)
        {
            mIsSelected = true;
            mHit.collider.GetComponent<BuildingSystem>().Building.ShowPopupUI();
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
        obj.AddComponent<BuildingSystem>().CreateBuildingData(name);
        GridBuildingSystem.gridSystemScript.InitializeBuliding(obj.GetComponent<BuildingSystem>());
        obj.GetComponent<BuildingSystem>().Building.BuildingData.position = obj.transform.position;
        myBuildings.buildings.Add(obj.GetComponent<BuildingSystem>().Building);
        foreach(Building b in myBuildings.buildings)
        {
            Debug.Log(b.BuildingData.id);
        }
        playerData.buildingNum++;
    }

    public void Click()
    {
        foreach (BuildingData s in myBuildingsData.buildings)
        {
            Debug.Log(s.id + " " + s.position);
        }
    }

    void FirstMapOn() // �ƹ��͵� ���� maintilemap�� �������ִ� �Լ�
    {
        if (PlayerData.buildingNum == 0)  GridBuildingSystem.gridSystemScript.SaveTileMap();
    }
}
