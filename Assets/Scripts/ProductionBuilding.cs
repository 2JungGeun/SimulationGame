using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class ProductionBuildingData
{
    public int yield;
    public int currentCapacity;
    public int unitNum;
    ProductionBuildingData()
    {
        unitNum = 0;
    }
}

public class ProductionBuilding : Building
{
    private ProductionBuildingData data;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
