using UnityEngine;
[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]

public class ItemSO : ScriptableObject
{
    public Sprite icon;
    public string itemName;
    public ItemRarity rarity;

    public int AttackPower;
    public int DefensePower;
}
