using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptble Object/ItemData")]
public class ItemData : ScriptableObject {
	public enum ItemTypeEnum  { Melee, Range, Glove, Shoe, Heal }

	[Header("# Main Info")]
	[SerializeField] private ItemTypeEnum itemType;

	[SerializeField] private int itemId;
	[SerializeField] private string itemName;
	[SerializeField] private string itemDesc;
	[SerializeField] private Sprite itemIcon;

	[Header("# Level Data")]
	[SerializeField] private float baseDamge;

	[SerializeField] private int baseCount;
	[SerializeField] private float[] damages;
	[SerializeField] private int[] counts;

	[Header("# Weapon")]
	[SerializeField] private GameObject projectile;

	public ItemTypeEnum ItemType => itemType;

	public int ItemId => itemId;

	public string ItemName => itemName;

	public string ItemDesc => itemDesc;

	public Sprite ItemIcon => itemIcon;

	public float BaseDamge => baseDamge;

	public int BaseCount => baseCount;

	public float[] Damages => damages;

	public int[] Counts => counts;

	public GameObject Projectile => projectile;
}