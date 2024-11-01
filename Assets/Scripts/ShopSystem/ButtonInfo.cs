using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonInfo : MonoBehaviour
{
    public int ItemID;
    public TextMeshProUGUI PriceTxt;
    public TextMeshProUGUI QuantityTxt;
    public GameObject ShopManager;

    // Update is called once per frame
    void Update()
    {
        PriceTxt.text = ": $" + ShopManager.GetComponent<ShopManagerScript>().shopItem[2, ItemID].ToString();
        QuantityTxt.text = ShopManager.GetComponent<ShopManagerScript>().shopItem[3, ItemID].ToString();
    }
}
