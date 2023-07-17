using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textName;
    [SerializeField]
    private TMP_Text textPrice;
    public GameObject bs;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(this.buy);
    }

    public void Setup(BuildingPrice buildingPrice)
    {
        textName.text = buildingPrice.name;
        textPrice.text = buildingPrice.price;
    }

    public void buy()
    {
        Int32.TryParse(textPrice.text, out int price);
        Debug.Log(textName.text + "," + textPrice.text);
        UIManager.GetUIManager().HideShopUI();
        GameManager.GetGameManager().BuyBuildingInStore(textName.text, price);
    }
}
