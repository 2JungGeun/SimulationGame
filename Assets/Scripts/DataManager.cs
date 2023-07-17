using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

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
    [SerializeField]
    private TextAsset buildingPriceData;
    [SerializeField]
    private TextAsset defenseBuildingData;
    [SerializeField]
    private TextAsset productionBuildingData;
    [SerializeField]
    private TextAsset buildingData;

    private string playerDataPath;
    private string buildingDataPath;
    private string buildingDataFileName;

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
        playerDataPath = Path.Combine(Application.dataPath, "Database/PlayerData.json");
        buildingDataPath = Path.Combine(Application.dataPath, "Database/BuildingData.dat");
        LoadBaseData();
    }

    public void SavePlayerDataToJson()
    {
        string jsonData = JsonUtility.ToJson(GameManager.GetGameManager().PlayerData);
        File.WriteAllText(playerDataPath, jsonData);
    }
    
    public void LoadPlayerDataFromJson()
    {
        if (!File.Exists(playerDataPath))
            SavePlayerDataToJson();
        string jsonData = File.ReadAllText(playerDataPath);
        GameManager.GetGameManager().PlayerData = JsonUtility.FromJson<PlayerData>(jsonData);
    }

    public void LoadBuildingDataFromBinaryfile()
    {
        if (!File.Exists(buildingDataPath))
            SaveBuildingDataToBinaryfile();
        FileStream fileStream = new FileStream(buildingDataPath, FileMode.Open);
        IFormatter formatter = new BinaryFormatter();
        GameManager.GetGameManager().MyBuildings = formatter.Deserialize(fileStream) as MyBuilding;
        fileStream.Close();
    }

    public void SaveBuildingDataToBinaryfile()
    {
        IFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(buildingDataPath, FileMode.Create);
        formatter.Serialize(fileStream, GameManager.GetGameManager().MyBuildings);
        fileStream.Close();
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

        foreach(string line in buildings)
        {
            string[] row = line.Split('\t');
            buildingDataDic.Add(row[1], new BuildingData(row[0], row[1], row[2], row[3], row[4]));
        }

        string[] defenseBuildings = defenseBuildingData.text.Substring(0, defenseBuildingData.text.Length - 1).Split("\n");
        foreach (string building in defenseBuildings)
        {
            string[] row = building.Split('\t');
            defenseBuildingDataDic.Add(row[0], new DefenseBuildingData(row[1], row[2], row[3]));
        }

        string[] productionBuildings = productionBuildingData.text.Substring(0, productionBuildingData.text.Length - 1).Split("\n");
        foreach (string building in productionBuildings)
        {
            string[] row = building.Split('\t');
            productionBuildingDataDic.Add(row[0], new ProductionBuildingData(row[1], row[2]));
        }
    }
}
