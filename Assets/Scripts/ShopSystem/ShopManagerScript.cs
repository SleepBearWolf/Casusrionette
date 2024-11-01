using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ShopManagerScript : MonoBehaviour
{
    public int[,] shopItem = new int[5, 5];
    public float coins;
    public TextMeshProUGUI CoinText;

    public PlayerInventory playerInventory; // ���������ҧ�ԧ�֧ PlayerInventory
    public ItemBaseData appleData;
    public ItemBaseData bananaData;
    public ItemBaseData orangeData;
    public ItemBaseData chickenData;

    void Start()
    {
        // ��Ǩ�ͺ��ҵ�駤�� CoinText � Inspector �������
        if (CoinText != null)
        {
            CoinText.text = "Coins: " + coins.ToString();
        }
        else
        {
            Debug.LogWarning("CoinText is not assigned in the Inspector.");
        }

        // ID's
        shopItem[1, 1] = 1;
        shopItem[1, 2] = 2;
        shopItem[1, 3] = 3;
        shopItem[1, 4] = 4;

        // Price
        shopItem[2, 1] = 2;
        shopItem[2, 2] = 4;
        shopItem[2, 3] = 6;
        shopItem[2, 4] = 9;

        // Quantity
        shopItem[3, 1] = 0;
        shopItem[3, 2] = 0;
        shopItem[3, 3] = 0;
        shopItem[3, 4] = 0;
    }

    public void Buy()
    {
        // ��Ǩ�ͺ��� EventSystem ��� ButtonRef �ա�õ�駤��
        GameObject ButtonRef = GameObject.FindGameObjectWithTag("Event").GetComponent<EventSystem>()?.currentSelectedGameObject;
        if (ButtonRef == null)
        {
            Debug.LogWarning("ButtonRef is null. Make sure you have an EventSystem and the button has been clicked.");
            return;
        }

        // ��Ǩ�ͺ��� ButtonRef �� ButtonInfo component
        ButtonInfo buttonInfo = ButtonRef.GetComponent<ButtonInfo>();
        if (buttonInfo == null)
        {
            Debug.LogWarning("ButtonInfo component is missing on the selected button.");
            return;
        }

        int itemID = buttonInfo.ItemID;

        if (coins >= shopItem[2, itemID])
        {
            coins -= shopItem[2, itemID];
            shopItem[3, itemID]++;
            if (CoinText != null)
            {
                CoinText.text = "Coins: " + coins.ToString();
            }
            buttonInfo.QuantityTxt.text = shopItem[3, itemID].ToString();

            // �������������� PlayerInventory
            ItemBaseData itemData = GetItemDataByID(itemID);
            if (itemData != null && playerInventory != null && playerInventory.AddItem(itemData))
            {
                Debug.Log("Item added to inventory: " + itemData.itemName);
            }
            else
            {
                Debug.LogWarning("Inventory is full or item data not found or PlayerInventory is not assigned.");
            }
        }
    }

    private ItemBaseData GetItemDataByID(int itemID)
    {
        // �֧������������ҡ�����ҧ�ԧ� Inspector
        switch (itemID)
        {
            case 1:
                return appleData;
            case 2:
                return bananaData;
            case 3:
                return orangeData;
            case 4:
                return chickenData;
            default:
                return null;
        }
    }
}
