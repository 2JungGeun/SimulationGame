using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization;

[System.Serializable]
public class DefenseBuildingData
{
    public int damage;
    public int attackSpeed;
    public int range;
    public int unitID;

    public DefenseBuildingData() {}

    public DefenseBuildingData DeepCopy() // 건물 생성 시 호출
    {
        DefenseBuildingData data = new DefenseBuildingData();
        data.damage = this.damage;
        data.attackSpeed = this.attackSpeed;
        data.range = this.range;
        data.unitID = -1;
        return data;
    }

    public DefenseBuildingData(string damage, string attackSpeed, string range) 
    {
        Int32.TryParse(damage, out this.damage);
        Int32.TryParse(attackSpeed, out this.attackSpeed);
        Int32.TryParse(range, out this.range);
        unitID = -1;
    }
}

[Serializable]
public class DefenseBuilding : Building
{
    [SerializeField]
    private DefenseBuildingData defenseBuildingData = new DefenseBuildingData();
    public DefenseBuildingData DefenseBuildingData { get { return defenseBuildingData; } set { defenseBuildingData = value; } }
    private int maxLevel = 5;
    // Start is called before the first frame update

    public DefenseBuilding() { }

    public DefenseBuilding(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        defenseBuildingData.damage = (int)info.GetValue("damage", typeof(int));
        defenseBuildingData.attackSpeed = (int)info.GetValue("attackSpeed", typeof(int));
        defenseBuildingData.range = (int)info.GetValue("range", typeof(int));
        defenseBuildingData.unitID = (int)info.GetValue("unitID", typeof(int));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("damage", defenseBuildingData.damage);
        info.AddValue("attackSpeed", defenseBuildingData.attackSpeed);
        info.AddValue("range", defenseBuildingData.range);
        info.AddValue("unitID", defenseBuildingData.unitID);
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
