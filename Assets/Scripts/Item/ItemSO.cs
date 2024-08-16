using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public ItemRarity rarity;
    public Sprite icon;
    public EquipItem equipStat;

    [SerializeField]
    private string itemID;

    public string ItemID => itemID;

    public void OnValidate()
    {
        if (string.IsNullOrEmpty(itemID))
        {
            itemID = System.Guid.NewGuid().ToString();
        }
    }
}
