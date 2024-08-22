using System.Collections.Generic;
using System.Linq;

using Unity.VisualScripting;

using UnityEngine;

#pragma warning disable CS0219 // 변수가 할당되었지만 해당 값이 사용되지 않았습니다.
public class PoolManager : MonoBehaviour {
	[Header("Enemy")]
	[SerializeField] private GameObject[] darkElfPrefabs;

	[SerializeField] private GameObject[] humanPrefabs;
	[SerializeField] private GameObject[] witchPrefabs;
	[SerializeField] private GameObject[] orcPrefabs;
	[SerializeField] private GameObject[] skeletonPrefabs;

	[Header("Weapon")]
	[SerializeField] private GameObject[] weaponPrefabs;

	[Header("Bullet")]
	[SerializeField] private GameObject[] bulletPrefabs;

	private Dictionary<string, List<GameObject>> pools;
	private Dictionary<string, GameObject[]> prefabDictionary;

	private void Awake() {
		// 프리팹 그룹을 딕셔너리에 저장
		prefabDictionary = new Dictionary<string, GameObject[]> {
			{ "DarkElf", darkElfPrefabs },
			{ "Human", humanPrefabs },
			{ "Witch", witchPrefabs },
			{ "Orc", orcPrefabs },
			{ "Skeleton", skeletonPrefabs },
			{ "Weapon", weaponPrefabs },
			{ "Bullet", bulletPrefabs }
		};

		// 풀 초기화
		pools = new Dictionary<string, List<GameObject>>();

		foreach (var key in prefabDictionary.Keys) {
			pools[key] = new List<GameObject>();
		}
	}

	public GameObject GetObject(string category, int index) {
		// 카테고리와 인덱스를 사용해 풀에 있는 비활성화된 오브젝트 찾기
		var pool = pools[category];
		var prefabList = prefabDictionary[category];

		GameObject select = pool.FirstOrDefault(item => !item.activeSelf);

		// 비활성화된 오브젝트가 없으면 새로 생성
		if (select == null) {
			if (index >= 0 && index < prefabList.Length) {
				select = Instantiate(prefabList[index], transform);
				
				pool.Add(select);
			} else {
				Debug.LogError("Invalid index for prefab array.");

				return null;
			}
		}

		select.SetActive(true);

		return select;
	}

	public GameObject[] GetPrefabs(string category) {
		return prefabDictionary.ContainsKey(category) ? prefabDictionary[category] : null;
	}

	public GameObject[] DarkElfPrefabs => darkElfPrefabs;

	public GameObject[] HumanPrefabs => humanPrefabs;

	public GameObject[] WitchPrefabs => witchPrefabs;

	public GameObject[] OrcPrefabs => orcPrefabs;

	public GameObject[] SkeletonPrefabs => skeletonPrefabs;

	public GameObject[] WeaponPrefabs => weaponPrefabs;

	public GameObject[] BulletPrefabs => bulletPrefabs;
}