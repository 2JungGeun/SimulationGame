using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System;
using UnityEditorInternal;

[System.Serializable]
public class BuildingData 
{
    public string id;
    public string type;
    public string name;
    public int hp;
    public int currentLevel;
    public int upgradeCost;
    public BoundsInt area;
    public Vector3 position;
    public bool isPlaced = false;
    public BuildingData()
    {
        currentLevel = 1;
        position = new Vector3();
    }
    public BuildingData DeepCopy()
    {
        BuildingData data = new BuildingData();
        data.id = this.id;
        data.type = this.type;
        data.name = this.name;
        data.hp = this.hp;
        data.currentLevel = this.currentLevel;
        data.upgradeCost = this.upgradeCost;
        data.area.size = new Vector3Int(2, 2, 1);
        data.position = this.position;
        data.isPlaced = this.isPlaced;
        return data;
    }

    public BuildingData(string type, string name, string hp, string upgradeCost)
    {
        this.type = type;
        this.name = name;
        Int32.TryParse(hp, out this.hp);
        this.currentLevel = 1;
        Int32.TryParse(upgradeCost, out this.upgradeCost);
        area.size = new Vector3Int(2, 2, 1);
        position = new Vector3();
        this.isPlaced = false;
    }

}

public class Building : MonoBehaviour
{
    private Sprite[] sprite;
    [SerializeField]
    public BuildingData buildingData = new BuildingData();
    private int maxLevel = 5;

    public BuildingData BuildingData { get { return buildingData; } set { buildingData = value; } }
   
    // Start is called before the first frame update
    void Start()
    {
        modifyUpgradeCost();
    }

    private void modifyUpgradeCost()
    {
        buildingData.upgradeCost += (buildingData.currentLevel - 1) * 300;
    }

    public void Upgrade()
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
    public void ShowPopupUI()
    {
        UIManager.GetUIManager().ShowBuildingPopupUI(ref buildingData);
        UIManager.GetUIManager().UpgradeButton.onClick.AddListener(this.Upgrade);
        GameManager.GetGameManager().isBuildingUIOn = true;
    }

    private void OnMouseDrag()
    {
        if (buildingData.isPlaced) return;

        if (GameManager.GetGameManager().isBuildingUIOn) return;

        Debug.Log("onmousedragOn");

        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        //Vector3 prevPosition = transform.position;
        transform.position = objPosition;
        GridBuildingSystem.gridSystemScript.ShowBuildingArea();
        transform.position = GridBuildingSystem.gridSystemScript.gridLayout.CellToWorld(buildingData.area.position);
        Int32.TryParse(buildingData.id, out int index);
        GameManager.GetGameManager().MyBuildingsData.buildings[index].position = transform.position;
        //if(prevPosition != transform.position)
        //    UIManager.GetUIManager().HideBuildingPopupUI();
    }
}
