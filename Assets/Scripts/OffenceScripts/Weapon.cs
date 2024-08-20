using System;

using UnityEngine;

#pragma warning disable CS0414 // 필드가 대입되었으나 값이 사용되지 않습니다

public class Weapon : MonoBehaviour {
	[SerializeField] private int id;
	[SerializeField] private int prefabId;
	[SerializeField] private float damage;
	[SerializeField] private int count;
	[SerializeField] private float speed;

	private float timer;

	private GameManager gameManager;
	private Player player;
	private PoolManager poolManager;

	private void Awake() {
		gameManager = GameManager.Instance;
		player = gameManager.Player[gameManager.PlayerId];
		poolManager = gameManager.PoolManager;
	}

	private void Update() {
		if (!gameManager.IsLive) return;

		if (player.gameObject.name == "Dummy")
			player = gameManager.Player[gameManager.PlayerId];

		switch (id) {
			case 0:
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
				transform.Rotate(Vector3.back * (speed * Time.deltaTime));

				break;

			default:
				timer += Time.deltaTime;

				if (timer > speed) {
					timer = 0f;
					CustomLogger.Log("fire 실행");

					Fire();
				}

				break;
		}
	}

	public void LevelUp(float damage, int count) {
		this.damage = damage * Character.Damage;
		this.count += count;

		if (id == 0 || id == 11 || id == 12 || id == 13 || id == 14 || id == 15 || id == 16) {
			Batch();
		}

		player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
	}

	public void Initialized(ItemData itemData) {
		// Basic Set
		int num = itemData.ItemId;

		name = "Weapon" + num;

		transform.parent = player.transform;
		transform.localPosition = Vector3.zero;

		// Property Set
		id = num;
		damage = itemData.BaseDamge * Character.Damage;
		count = itemData.BaseCount + Character.Count;

		for (int i = 1; i < poolManager.WeaponPrefabs.Length; i++) {
			if (itemData.Projectile == poolManager.WeaponPrefabs[i]) {
				prefabId = i;

				break;
			}
		}

		// prefabId = num;

		switch (id) {
			case 0:
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
				speed = 150 * Character.WeaponRate;

				Batch();

				break;

			default:
				speed = 0.5f * Character.WeaponRate;

				break;
		}

		// Hand Set
		Hand hand = player.Hands[(int)itemData.ItemType];

		if (hand.SpriteRenderer == null) {
			CustomLogger.LogError("SpriteRenderer is not assigned in Hand script!");

			return;
		}

		hand.SpriteRenderer.sprite = itemData.Hand;
		hand.gameObject.SetActive(true);

		player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
	}

	private void Batch() {
		for (int i = 0; i < count; i++) {
			Transform bullet;

			if (i < transform.childCount) {
				bullet = transform.GetChild(i);
			} else {
				bullet = poolManager.GetWeapon(prefabId).transform;
				bullet.parent = transform;
			}

			bullet.localPosition = Vector3.zero;
			bullet.localRotation = Quaternion.identity;

			Vector3 rotateVector3 = Vector3.forward * 360 * i / count;

			bullet.Rotate(rotateVector3);
			bullet.Translate(bullet.up * 1.5f, Space.World);

			bullet.GetComponent<Bullet>().Initialized(damage, -100, Vector3.zero); // -1 is Infinity penetration.

			AudioManager.Instance.PlaySfx(AudioManager.Sfx.Melee);
		}
	}

	private void Fire() {
		if (!player.Scanner.NearestTarget) return;

		Vector3 targetPosition = player.Scanner.NearestTarget.position;
		Vector3 direction = targetPosition - transform.position;

		direction = direction.normalized;

		Transform bullet = poolManager.GetFire(prefabId).transform;

		bullet.position = transform.position;
		bullet.rotation = Quaternion.FromToRotation(Vector3.up, direction);

		bullet.GetComponent<Bullet>().Initialized(damage, count, direction); // -1 is Infinity penetration.

		AudioManager.Instance.PlaySfx(AudioManager.Sfx.Range);
	}

	public int ID => id;

	public int PrefabId => prefabId;

	public float Damage => damage;

	public int Count => count;

	public float Speed {
		get => speed;

		set => speed = value;
	}
}