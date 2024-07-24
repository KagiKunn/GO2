using System;

using UnityEngine;

public class Gear : MonoBehaviour {
	private ItemData.ItemTypeEnum type;
	private float rate;

	public void Initialized(ItemData itemData) {
		// Basic set
		name = "Gear " + itemData.ItemId;

		transform.parent = GameManager.Instance.Player.transform;
		transform.position = Vector3.zero;

		// Property Set
		type = itemData.ItemType;
		rate = itemData.Damages[0];

		ApplyGear();
	}

	public void LevelUp(float rate) {
		this.rate = rate;

		ApplyGear();
	}

	private void ApplyGear() {
		switch (type) {
			case ItemData.ItemTypeEnum.Glove:
				RateUp();

				break;

			case ItemData.ItemTypeEnum.Shoe:
				SpeedUp();

				break;
		}
	}

	private void RateUp() {
		Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

		foreach (Weapon weapon in weapons) {
			switch (weapon.ID) {
				case 0:
					weapon.Speed = 150 + (150 * rate);

					break;

				default:
					weapon.Speed = 0.5f * (1f - rate);

					break;
			}
		}
	}

	private void SpeedUp() {
		float speed = 3;

		GameManager.Instance.Player.Speed = speed + speed * rate;
	}
}