using System;

using UnityEngine;

public class Gear : MonoBehaviour {
	private ItemData.ItemTypeEnum type;
	private float rate;

	public void Initialized(ItemData itemData) {
		// Basic set
		name = "Gear " + itemData.ItemId;

		transform.parent = GameManager.Instance.Player[GameManager.Instance.PlayerId].transform;
		
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
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
					float speed = GameManager.Instance.AttackSpeed * Character.WeaponRate;

					weapon.Speed = speed + (speed * rate);

					break;

				default:
					speed = 0.5f * Character.WeaponRate;

					weapon.Speed = speed * (1f - rate);

					break;
			}
		}
	}

	private void SpeedUp() {
		float speed = 3 * Character.Speed;

		GameManager.Instance.Player[GameManager.Instance.PlayerId].Speed = speed + speed * rate;
	}
}