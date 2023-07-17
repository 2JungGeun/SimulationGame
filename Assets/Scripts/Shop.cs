using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField]
    private GameObject buildingUIpref;
    [SerializeField]
    private Transform buildingListUIParent;
    // Start is called before the first frame update
    void Start()
    {
        CreateBuildingListShopUI();
    }

    private void CreateBuildingListShopUI()
    {
        foreach (BuildingPrice buildingPrice in DataManager.GetDataManager().BuildingPricesList)
        {
            GameObject buildingUI = Instantiate(buildingUIpref, buildingListUIParent);
            buildingUI.GetComponent<BuildingUI>().Setup(buildingPrice);
        }
    }

}
