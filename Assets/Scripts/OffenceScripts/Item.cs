using System;

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Item : MonoBehaviour {
	[SerializeField] private ItemData itemData;
	[SerializeField] private int level;
	[SerializeField] private Weapon weapon;
	[SerializeField] private Gear gear;

	private Image icon;
	private Text textLevel;

	private void Awake() {
		icon = GetComponentsInChildren<Image>()[1];
		icon.sprite = itemData.ItemIcon;

		Text[] texts = GetComponentsInChildren<Text>();
		textLevel = texts[0];
	}

	private void LateUpdate() {
		textLevel.text = "Lv." + (level + 1);
	}

	public void OnClick() {
		switch (itemData.ItemType) {
			case ItemData.ItemTypeEnum.Melee:
			case ItemData.ItemTypeEnum.Range:
				if (level == 0) {
					GameObject newWeapon = new GameObject();

					weapon = newWeapon.AddComponent<Weapon>();

					weapon.Initialized(itemData);
				} else {
					float nextDamage = itemData.BaseDamge;
					int nextCount = 0;

					nextDamage += itemData.BaseDamge * itemData.Damages[level];
					nextCount += itemData.Counts[level];

					weapon.LevelUp(nextDamage, nextCount);
				}

				level++;

				break;

			case ItemData.ItemTypeEnum.Glove:
			case ItemData.ItemTypeEnum.Shoe:
				if (level == 0) {
					GameObject newGear = new GameObject();

					gear = newGear.AddComponent<Gear>();

					gear.Initialized(itemData);
				} else {
					float nextRate = itemData.Damages[level];

					gear.LevelUp(nextRate);
				}

				level++;

				break;

			case ItemData.ItemTypeEnum.Heal:
				GameManager.Instance.Health = GameManager.Instance.MaxHealth;

				break;
		}

		if (level == itemData.Damages.Length) {
			GetComponent<Button>().interactable = false;
		}
	}
}