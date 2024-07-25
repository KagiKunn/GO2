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

	private Player player;
	private PoolManager poolManager;

	private void Awake() {
		player = GameManager.Instance.Player;
		poolManager = GameManager.Instance.PoolManager;
	}

	private void Update() {
		switch (id) {
			case 0:
				transform.Rotate(Vector3.back * speed * Time.deltaTime);

				break;

			default:
				timer += Time.deltaTime;

				if (timer > speed) {
					timer = 0f;

					Fire();
				}

				break;
		}

		// Test Code
		if (Input.GetButtonDown("Jump")) LevelUp(10, 1);
	}

	public void LevelUp(float damage, int count) {
		this.damage = damage;
		this.count += count;

		if (id == 0) {
			Batch();
		}
		
		player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
	}

	public void Initialized(ItemData itemData) {
		// Basic Set
		name = "Weapon" + itemData.ItemId;

		transform.parent = player.transform;
		transform.localPosition = Vector3.zero;

		// Property Set
		id = itemData.ItemId;
		damage = itemData.BaseDamge;
		count = itemData.BaseCount;

		for (int i = 0; i < poolManager.Prefabs.Length; i++) {
			if (itemData.Projectile == poolManager.Prefabs[i]) {
				prefabId = i;

				break;
			}
		}

		switch (id) {
			case 0:
				speed = 150;

				Batch();

				break;

			default:
				speed = 0.4f;

				break;
		}
		
		player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
	}

	private void Batch() {
		for (int i = 0; i < count; i++) {
			Transform bullet;

			if (i < transform.childCount) {
				bullet = transform.GetChild(i);
			} else {
				bullet = poolManager.Get(prefabId).transform;
				bullet.parent = transform;
			}

			bullet.localPosition = Vector3.zero;
			bullet.localRotation = Quaternion.identity;

			Vector3 rotateVector3 = Vector3.forward * 360 * i / count;

			bullet.Rotate(rotateVector3);
			bullet.Translate(bullet.up * 1.5f, Space.World);

			bullet.GetComponent<Bullet>().Initialized(damage, -1, Vector3.zero); // -1 is Infinity penetration.
		}
	}

	private void Fire() {
		if (!player.Scanner.NearestTarget) return;

		Vector3 targetPosition = player.Scanner.NearestTarget.position;
		Vector3 direction = targetPosition - transform.position;

		direction = direction.normalized;

		Transform bullet = poolManager.Get(prefabId).transform;

		bullet.position = transform.position;
		bullet.rotation = Quaternion.FromToRotation(Vector3.up, direction);

		bullet.GetComponent<Bullet>().Initialized(damage, count, direction); // -1 is Infinity penetration.
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