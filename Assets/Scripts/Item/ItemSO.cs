using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public ItemRarity rarity;
    public Sprite icon;
    public EquipItem equipStat;
}
