using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Inventory : MonoBehaviour
{
    public GameObject slotPrefab;
    public const int numSlots = 5;
    Image[] itemImages = new Image[numSlots];
    Item[] items = new Item[numSlots];
    GameObject[] slots = new GameObject[numSlots];

    public void CreateSlots()
    {
        if (slotPrefab != null)
        {
            for (int i = 0; i < numSlots; i++)
            {
                GameObject newSlot = Instantiate(slotPrefab);
                newSlot.name = "ItemSlot_" + i;
                newSlot.transform.SetParent(gameObject.transform.GetChild(0).transform);
                slots[i] = newSlot;
                itemImages[i] = newSlot.transform.GetChild(1).GetComponent<Image>();
            }
        }
    }

    public bool AddItem(Item itemToAdd)
    {
        for (int i = 0; i < items.Length; i++)
        {
            //stackable 아이템이 이미 인벤토리에 존재하는 경우, 기존 슬릇에 추가한다.
            if ((items[i] != null) && (items[i].itemType == itemToAdd.itemType) && (itemToAdd.stackable == true)) 
            {
                items[i].quantity += 1;

                Slot slotScript = slots[i].gameObject.GetComponent<Slot>();
                TextMeshProUGUI quantityText = slotScript.qtyText;
                quantityText.enabled = true;
                quantityText.text = items[i].quantity.ToString();

                return true;
            }

            // 아이템이 이미 있는 칸을 다 지나치고 빈칸에 도달한 경우
            if (items[i] == null)
            {
                items[i] = Instantiate(itemToAdd);
                items[i].quantity = 1;
                itemImages[i].sprite = itemToAdd.sprite;
                itemImages[i].enabled = true;

                return true;
            }
        }
        return false;
    }

    public void Start()
    {
        CreateSlots();
    }
}
