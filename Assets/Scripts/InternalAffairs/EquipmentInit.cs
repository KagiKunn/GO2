using System;
using UnityEngine;

public class EquipmentInit : MonoBehaviour
{
    private GameObject inventoryObj;

    private void Start()
    {
        inventoryObj = GameObject.Find("InventoryCanvas");
    }

    public void EquipmentBtn()
    {
        CustomLogger.Log("inventory!!");
        Canvas inventory = inventoryObj.GetComponent<Canvas>();
        inventory.enabled = !inventory.enabled;
    }
}
