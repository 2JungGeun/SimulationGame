using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    private Sprite[] sprite;
    [SerializeField]
    private Building building = null;
    public Building Building { get { return building; } set { building = value; } }
    // Start is called before the first frame update
    void Start()
    {
        building.BuildingData.position = GridBuildingSystem.current.gridLayout.CellToWorld(building.BuildingData.area.position);
        this.gameObject.transform.position = building.BuildingData.position;
        building.modifyUpgradeCost();
    }

    public void CreateBuildingData(string name)
    {
        BuildingData data = DataManager.GetDataManager().BuildingDataDic[name].DeepCopy();
        data.id = GameManager.GetGameManager().PlayerData.buildingNum;
        if (data.type == "defense")
        {
            this.building = new DefenseBuilding();
            building.BuildingData = data;
            ((DefenseBuilding)building).DefenseBuildingData = DataManager.GetDataManager().DefeseBuildingDataDic[name].DeepCopy();
        }
        else if (data.type == "production")
        {
            this.building = new ProductionBuilding();
            building.BuildingData = data;
            ((ProductionBuilding)building).ProductionBuildingData = DataManager.GetDataManager().ProductionBuildingDataDic[name].DeepCopy();
        }
    }

    private void OnMouseDrag()
    {
        if (building.BuildingData.isPlaced)
            return;

        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = objPosition;
        GridBuildingSystem.current.FollowBuilding();
        //transform.position = buildingData.position;
        transform.position = GridBuildingSystem.current.gridLayout.CellToWorld(building.BuildingData.area.position);
        GameManager.GetGameManager().MyBuildings.buildings[building.BuildingData.id].BuildingData.position = transform.position;
    }

    public bool Installable()
    {
        Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = building.BuildingData.area;
        areaTemp.position = positionInt;

        if (GridBuildingSystem.current.CanTakeArea(areaTemp))
        {
            return true;
        }

        return false;
    }

    public void Install()
    {
        Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = building.BuildingData.area;
        areaTemp.position = positionInt;
        building.BuildingData.isPlaced = true;
        GridBuildingSystem.current.TakeArea(areaTemp);
    }
}
