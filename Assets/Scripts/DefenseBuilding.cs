using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public class DefenseBuildingData
{
    public int damage;
    public int attackSpeed;
    public DefenseBuildingData(string damage, string attackSpeed) 
    {
        Int32.TryParse(damage, out this.damage);
        Int32.TryParse(attackSpeed, out this.attackSpeed);
    }
}

public class DefenseBuilding : Building
{
    public DefenseBuildingData data;
    // Start is called before the first frame update
    void Start()
    {
        DataManager.GetDataManager().DefeseBuildingDataDic.TryGetValue(this.BuildingData.name, out data);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
