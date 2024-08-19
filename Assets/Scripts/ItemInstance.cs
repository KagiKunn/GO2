using System;
using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    public string uniqueID;
    public string itemID;
    [NonSerialized] public ItemSO itemData;

    public ItemInstance(ItemSO itemData)
    {
        this.itemData = itemData;
        this.uniqueID = Guid.NewGuid().ToString();
        this.itemID = itemData.ItemID;
    }

    public void LoadItemData()
    {
        CustomLogger.Log("로드 메서드 실행완료");
        string[] categories  =
            { "Armor", "Arrow", "Boots", "Bow", "Gloves", "Helmet", "Shield", "Two-handed sword", "Wand,Staff" };

        foreach (string category  in categories)
        {
            ItemSO[] itemsInCategory = Resources.LoadAll<ItemSO>($"Items/{category}");
            CustomLogger.Log($"Loading items from category: {category}");
            
            foreach (ItemSO item in itemsInCategory)
            {
                CustomLogger.Log($"Checking item: {item.itemName}, ID: {item.ItemID} against {this.itemID}");
                if (item.ItemID == this.itemID)
                {
                    this.itemData = item;
                    CustomLogger.Log($"Loaded item: {item.itemName} with ID {itemID}");
                    return;
                }
            }
        }
        CustomLogger.Log($"로드실패! with ID {this.itemID}");
    }
}
