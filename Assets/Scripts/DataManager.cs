using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.VersionControl;
using System;

[System.Serializable]
public class BuildingPrice
{
    public string name;
    public string price;
    public BuildingPrice(string name, string price)
    {
        this.name = name;
        this.price = price;
    }
}

public class DataManager : MonoBehaviour
{
    public static DataManager GetDataManager()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType<DataManager>();
            if (instance == null)
            {
                GameObject container = new GameObject("Data Manager");
                instance = container.AddComponent<DataManager>();
            }
        }
        return instance;
    }
    private static DataManager instance;
    public TextAsset buildingPriceData;
    public TextAsset defenseBuildingData;
    public TextAsset productionBuildingData;
    public TextAsset buildingData;

    private List<BuildingPrice> buildingPriceList = new List<BuildingPrice>();
    public List<BuildingPrice> BuildingPricesList { get { return buildingPriceList; } }

    private Dictionary<string, BuildingData> buildingDataDic = new Dictionary<string, BuildingData>();
    public Dictionary<string, BuildingData> BuildingDataDic { get { return buildingDataDic; } }

    private Dictionary<string, DefenseBuildingData> defenseBuildingDataDic = new Dictionary<string, DefenseBuildingData>();
    public Dictionary<string, DefenseBuildingData> DefeseBuildingDataDic { get { return defenseBuildingDataDic; } }

    private Dictionary<string, ProductionBuildingData> productionBuildingDataDic = new Dictionary<string, ProductionBuildingData>();
    public Dictionary<string, ProductionBuildingData> ProductionBuildingDataDic { get { return productionBuildingDataDic; } }

    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        LoadBaseData();
    }

    [ContextMenu("To Json Data")]
    public void SavePlayerDataToJson()
    {
        string jsonData = JsonUtility.ToJson(GameManager.GetGameManager().PlayerData);
        string path = Path.Combine(Application.dataPath, "PlayerData.json");
        File.WriteAllText(path, jsonData);
    }
    
    public void LoadPlayerDataFromJson()
    {
        string path = Path.Combine(Application.dataPath, "PlayerData.json");
        string jsonData = File.ReadAllText(path);
        GameManager.GetGameManager().PlayerData = JsonUtility.FromJson<PlayerData>(jsonData);
    }

    public void SaveBuildingDataToJson()
    {
        string jsonData = JsonUtility.ToJson(GameManager.GetGameManager().MyBuildingsData);
        string path = Path.Combine(Application.dataPath, "BuildingData.json");
        File.WriteAllText(path, jsonData);
    }

    public void LoadBuildingDataFromJson()
    {
        string path = Path.Combine(Application.dataPath, "BuildingData.json");
        string jsonData = File.ReadAllText(path);
        GameManager.GetGameManager().MyBuildingsData = JsonUtility.FromJson<MyBuildingData>(jsonData);
    }

    private void LoadBaseData()
    {
        string[] lines = buildingPriceData.text.Substring(0, buildingPriceData.text.Length - 1).Split("\n");
        foreach (string line in lines)
        {
            string[] row = line.Split('\t');
            buildingPriceList.Add(new BuildingPrice(row[0], row[1]));
        }
        string[] buildings = buildingData.text.Substring(0, buildingData.text.Length - 1).Split("\n");
        /*foreach (string line in buildings)
        {
            string[] row = line.Split('\t');
            buildingDataList.Add(new BuildingData(row[0], row[1], row[2]));
        }*/
        foreach(string line in buildings)
        {
            string[] row = line.Split('\t');
            buildingDataDic.Add(row[1], new BuildingData(row[0], row[1], row[2], row[3]));
        }

        string[] defenseBuildings = defenseBuildingData.text.Substring(0, defenseBuildingData.text.Length - 1).Split("\n");
        foreach (string building in defenseBuildings)
        {
            string[] row = building.Split('\t');
            defenseBuildingDataDic.Add(row[0], new DefenseBuildingData(row[1], row[2]));
        }
    }
}
