using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEditorInternal;
using System;
using System.Drawing;
using System.Runtime.Serialization;
using static UnityEngine.RuleTile.TilingRuleOutput;

[System.Serializable]
public class MyBuilding : ISerializable
{
    public List<Building> buildings;
    public MyBuilding()
    {
        buildings = new List<Building>();
    }
    MyBuilding(SerializationInfo info, StreamingContext context)
    {
        buildings = (List<Building>)info.GetValue("buildingList", typeof(List<Building>));
    }
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("buildingList", buildings);
    }
}

[System.Serializable]
public class BuildingData
{
    public int id;
    public string type;
    public string name;
    public int hp;
    public int currentLevel;
    public int upgradeCost;
    public BoundsInt area;
    public Vector3 position;
    public bool isPlaced;

    public BuildingData()
    {
        currentLevel = 1;
        position = new Vector3();
    }

    public BuildingData DeepCopy() // 건물 생성 시 호출
    {
        BuildingData data = new BuildingData();
        data.id = this.id;
        data.type = this.type;
        data.name = this.name;
        data.hp = this.hp;
        data.currentLevel = this.currentLevel;
        data.upgradeCost = this.upgradeCost;
        data.area.size = this.area.size;
        data.isPlaced = false;
        return data;
    }

    public BuildingData(string type, string name, string hp, string upgradeCost, string size) // Called when basic building data is loaded
    {
        this.type = type;
        this.name = name;
        Int32.TryParse(hp, out this.hp);
        this.currentLevel = 1;
        Int32.TryParse(upgradeCost, out this.upgradeCost);
        Int32.TryParse(size, out int sizeInt);
        area.size = new Vector3Int(sizeInt, sizeInt, 1);
        position = new Vector3();
        this.isPlaced = false;
    }

}

[System.Serializable]
public class Building : ISerializable
{
    [SerializeField]
    protected BuildingData buildingData = new BuildingData();
    public BuildingData BuildingData { get { return buildingData; } set { buildingData = value; } }

    public Building() { }

    public Building(SerializationInfo info, StreamingContext context)
    {
        buildingData.id = (int)info.GetValue("id", typeof(int));
        buildingData.type = (string)info.GetValue("type", typeof(string));
        buildingData.name = (string)info.GetValue("name", typeof(string));
        buildingData.hp = (int)info.GetValue("hp", typeof(int));
        buildingData.currentLevel = (int)info.GetValue("level", typeof(int));
        buildingData.upgradeCost = (int)info.GetValue("cost", typeof(int));
        int areaSize = (int)info.GetValue("areaSize", typeof(int));
        buildingData.area.size = new Vector3Int(areaSize, areaSize, 1);
        buildingData.area.position = new Vector3Int((int)info.GetValue("areaPosX", typeof(int)), (int)info.GetValue("areaPosY", typeof(int)), 0);
        buildingData.position = new Vector3((float)info.GetValue("positionX", typeof(float)), (float)info.GetValue("positionY", typeof(float)), 0);
        buildingData.isPlaced = (bool)info.GetValue("isPlaced", typeof(bool));
    }

    virtual public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("id", buildingData.id);
        info.AddValue("type", buildingData.type);
        info.AddValue("name", buildingData.name);
        info.AddValue("hp", buildingData.hp);
        info.AddValue("level", buildingData.currentLevel);
        info.AddValue("cost", buildingData.upgradeCost);
        info.AddValue("areaSize", buildingData.area.size.x, typeof(int));
        info.AddValue("areaPosX", buildingData.area.position.x, typeof(int));
        info.AddValue("areaPosY", buildingData.area.position.y, typeof(int));
        info.AddValue("positionX", buildingData.position.x, typeof(float));
        info.AddValue("positionY", buildingData.position.y, typeof(float));
        info.AddValue("isPlaced", buildingData.isPlaced);

    }

    public virtual void Upgrade() { }

    public void modifyUpgradeCost()
    {
        buildingData.upgradeCost += (buildingData.currentLevel - 1) * 300;
    }

    public void ShowPopupUI()
    {
        UIManager.GetUIManager().ShowBuildingPopupUI(ref buildingData);
        if (buildingData.type == "main")
            return;
        UIManager.GetUIManager().UpgradeButton.onClick.AddListener(this.Upgrade);
    }
}
