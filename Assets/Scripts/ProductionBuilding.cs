using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System;

[System.Serializable]
public class ProductionBuildingData
{
    public int yield;
    public int currentCapacity;
    public int unitNum;

    public ProductionBuildingData() { }

    public ProductionBuildingData DeepCopy() // 건물 생성 시 호출
    {
        ProductionBuildingData data = new ProductionBuildingData();
        data.yield = this.yield;
        data.currentCapacity = this.currentCapacity;
        data.unitNum = 0;
        return data;
    }
    public ProductionBuildingData(string yield, string capacity) : base()
    {
        Int32.TryParse(yield, out this.yield);
        Int32.TryParse(capacity, out this.currentCapacity);
        unitNum = 0;
    }
}

[Serializable]
public class ProductionBuilding : Building
{
    [SerializeField]
    private ProductionBuildingData productionBuildingData = new ProductionBuildingData();
    public ProductionBuildingData ProductionBuildingData { get { return productionBuildingData; } set { productionBuildingData = value; } }
    private int maxLevel = 5;

    public ProductionBuilding()
    {

    }

    public ProductionBuilding(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        productionBuildingData.yield = (int)info.GetValue("yield", typeof(int));
        productionBuildingData.currentCapacity = (int)info.GetValue("capacity", typeof(int));
        productionBuildingData.unitNum = (int)info.GetValue("unitNum", typeof(int));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("yield", productionBuildingData.yield);
        info.AddValue("capacity", productionBuildingData.currentCapacity);
        info.AddValue("unitNum", productionBuildingData.unitNum);
    }

    public override void Upgrade()
    {
        if (GameManager.GetGameManager().PlayerData.money < buildingData.upgradeCost) return;
        if (GameManager.GetGameManager().BuildingLimitLevel <= buildingData.currentLevel) return;
        if (maxLevel <= buildingData.currentLevel) return;
        GameManager.GetGameManager().UseMoney(buildingData.upgradeCost);
        buildingData.currentLevel++;
        modifyUpgradeCost();
        UIManager.GetUIManager().UpdateBuildingDataUI(ref buildingData);
        Debug.Log(GameManager.GetGameManager().PlayerData.money);
        if (GameManager.GetGameManager().PlayerData.money < buildingData.upgradeCost || GameManager.GetGameManager().BuildingLimitLevel <= buildingData.currentLevel)
        {
            Color color = UIManager.GetUIManager().UpgradeButton.image.color;
            color = Color.red;
            UIManager.GetUIManager().UpgradeButton.image.color = color;
        }
    }
}
