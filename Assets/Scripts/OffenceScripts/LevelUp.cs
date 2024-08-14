using System;

using UnityEngine;
using UnityEngine.Rendering;

using Random = UnityEngine.Random;

public class LevelUp : MonoBehaviour {
	private RectTransform rectTransform;
	private Item[] items;

	[SerializeField] private ItemData[] ItemDatas;

	private GameManager gameManager;

	private void Awake() {
		rectTransform = GetComponent<RectTransform>();
		items = GetComponentsInChildren<Item>(true);

		gameManager = GameManager.Instance;

		items[0].ItemData = ItemDatas[gameManager.PlayerId];
	}

	private void Update() {
		if (items[0].ItemData.name != "Item 10") return;

		items[0].ItemData = ItemDatas[gameManager.PlayerId];
		items[0].Level = items[gameManager.PlayerId + 5].Level;
		items[0].Weapon = items[gameManager.PlayerId + 5].Weapon;
		items[0].Gear = items[gameManager.PlayerId + 5].Gear;
		items[0].TextName = items[gameManager.PlayerId + 5].TextName;
		items[0].TextDesc = items[gameManager.PlayerId + 5].TextDesc;
		items[0].TextLevel = items[gameManager.PlayerId + 5].TextLevel;
	}

	public void Show(int playerId) {
		Next(playerId);

		rectTransform.localScale = Vector3.one;

		GameManager.Instance.Stop();

		AudioManager.Instance.PlaySfx(AudioManager.Sfx.LevelUp);

		AudioManager.Instance.EffectBgm(true);
	}

	public void Hide() {
		rectTransform.localScale = Vector3.zero;

		GameManager.Instance.Resume();

		AudioManager.Instance.PlaySfx(AudioManager.Sfx.Select);

		AudioManager.Instance.EffectBgm(false);
	}

	public void Select(int index) {
		items[index].OnClick();
	}

	public void Next(int playerId) {
		CustomLogger.Log(playerId + 4);

		// 1. 모든 아이템 비활성화
		foreach (Item item in items) {
			item.gameObject.SetActive(false);
		}

		// 2. 그 중에서 랜덤 3개 아이템 활성화
		int[] random = new int[3];

		while (true) {
			random[0] = Random.Range(0, 5);
			random[1] = Random.Range(0, 5);
			random[2] = Random.Range(0, 5);

			if (random[0] != random[1] && random[0] != random[2] && random[1] != random[2])
				break;
		}

		for (int i = 0; i < random.Length; i++) {
			Item randomItem = items[random[i]];

			// 3. 만렙 아이템의 경우는 소비 아이템으로 대체
			if (randomItem.Level == randomItem.ItemData.Damages.Length) {
				items[Random.Range(4, 5)].gameObject.SetActive(true);
			} else {
				randomItem.gameObject.SetActive(true);
			}
		}
	}
}