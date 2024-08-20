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
		pools = new List<GameObject>[enemyPrefabs.Length + weaponPrefabs.Length + bulletPrefabs.Length];

		for (int i = 0; i < pools.Length; i++) {
			pools[i] = new List<GameObject>();
		}
	}

	public GameObject Get(int index) {
		GameObject select = null;

		// ... 선택한 풀의 놀고 있는(비활성화 된) 게임 오브젝트 접근
		foreach (var item in pools[index].Where(item => !item.activeSelf))
		{
			// ... 발견하면 select 변수에 할당
			select = item;

			select.SetActive(true);

			break;
		}

		// ... 못 찾았으면?
		if (select.IsUnityNull()) {
			// ... 새롭게 생성하고 select 변수에 할당
			select = Instantiate(enemyPrefabs[index], transform);

			pools[index].Add(select);
		}

		return select;
	}
	
	public GameObject GetWeapon(int index) {
		GameObject select = null;

		// ... 선택한 풀의 놀고 있는(비활성화 된) 게임 오브젝트 접근
		foreach (var item in pools[index].Where(item => !item.activeSelf))
		{
			// ... 발견하면 select 변수에 할당
			select = item;

			select.SetActive(true);

			break;
		}

		// ... 못 찾았으면?
		if (select.IsUnityNull()) {
			// ... 새롭게 생성하고 select 변수에 할당
			select = Instantiate(weaponPrefabs[index], transform);

			pools[index].Add(select);
		}

		return select;
	}
	
	public GameObject GetFire(int index) {
		GameObject select = null;

		// ... 선택한 풀의 놀고 있는(비활성화 된) 게임 오브젝트 접근
		foreach (var item in pools[index].Where(item => !item.activeSelf))
		{
			// ... 발견하면 select 변수에 할당
			select = item;

			select.SetActive(true);

			break;
		}

		// ... 못 찾았으면?
		if (select.IsUnityNull()) {
			// ... 새롭게 생성하고 select 변수에 할당
			select = Instantiate(bulletPrefabs[index], transform);

			pools[index].Add(select);
		}

		return select;
	}

	public GameObject[] EnemyPrefabs => enemyPrefabs;

	public GameObject[] WeaponPrefabs => weaponPrefabs;

	public GameObject[] BulletPrefabs => bulletPrefabs;

	public List<GameObject>[] Pools => pools;
}