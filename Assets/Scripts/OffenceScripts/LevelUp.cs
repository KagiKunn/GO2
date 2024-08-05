using System;

using UnityEngine;

using Random = UnityEngine.Random;

public class LevelUp : MonoBehaviour {
	private RectTransform rectTransform;
	private Item[] items;

	private void Awake() {
		rectTransform = GetComponent<RectTransform>();
		items = GetComponentsInChildren<Item>(true);
	}

	public void Show() {
		Next();

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

	public void Next() {
		// 1. 모든 아이템 비활성화
		foreach (Item item in items) {
			item.gameObject.SetActive(false);
		}

		// 2. 그 중에서 랜덤 3개 아이템 활성화
		int[] random = new int[3];

		while (true) {
			random[0] = Random.Range(0, items.Length);
			random[1] = Random.Range(0, items.Length);
			random[2] = Random.Range(0, items.Length);

			if (random[0] != random[1] && random[0] != random[2] & random[1] != random[2])
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