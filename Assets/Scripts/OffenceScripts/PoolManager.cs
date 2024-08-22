using System.Collections.Generic;
using System.Linq;

using Unity.VisualScripting;

using UnityEngine;

#pragma warning disable CS0219 // 변수가 할당되었지만 해당 값이 사용되지 않았습니다.

public class PoolManager : MonoBehaviour {
	// .. 프리펩들을 보관할 변수
	[Header("Enemy")]
	[SerializeField] private GameObject[] enemyPrefabs;

	[Header("Weapon")]
	[SerializeField] private GameObject[] weaponPrefabs;

	[Header("Bullet")]
	[SerializeField] private GameObject[] bulletPrefabs;

	// .. 풀 담당을 하는 리스트들
	private List<GameObject>[] pools;

	private void Awake() {
		int totalLength = enemyPrefabs.Length + weaponPrefabs.Length + bulletPrefabs.Length;
		pools = new List<GameObject>[totalLength];

		for (int i = 0; i < pools.Length; i++) {
			pools[i] = new List<GameObject>();
		}
	}

	private int GetPoolIndex(int categoryIndex, int index) {
		switch (categoryIndex) {
			case 0: return index; // Enemy
			case 1: return enemyPrefabs.Length + index; // Weapon
			case 2: return enemyPrefabs.Length + weaponPrefabs.Length + index; // Bullet
			default: throw new System.ArgumentOutOfRangeException("Invalid category index");
		}
	}

	public GameObject GetObject(int categoryIndex, int index) {
		int poolIndex = GetPoolIndex(categoryIndex, index);
		GameObject select = null;

		foreach (var item in pools[poolIndex].Where(item => !item.activeSelf)) {
			select = item;
			select.SetActive(true);

			break;
		}

		if (select == null) {
			GameObject prefab = null;

			switch (categoryIndex) {
				case 0:
					prefab = enemyPrefabs[index];

					break;
				case 1:
					prefab = weaponPrefabs[index];

					break;
				case 2:
					prefab = bulletPrefabs[index];

					break;
			}

			select = Instantiate(prefab, transform);
			pools[poolIndex].Add(select);
		}

		return select;
	}

	public GameObject[] EnemyPrefabs => enemyPrefabs;

	public GameObject[] WeaponPrefabs => weaponPrefabs;

	public GameObject[] BulletPrefabs => bulletPrefabs;

	public List<GameObject>[] Pools => pools;
}