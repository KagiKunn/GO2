using UnityEngine;

public class GachaBoxController : MonoBehaviour {
	private Animator animator;
	public Gacha gacha;
	private bool isMultiGacha = false;
	private int gachaCount = 1;
	public ParticleSystem sparkleEffect;

	private void Start() {
		animator = GetComponent<Animator>();

		if (sparkleEffect != null) {
			sparkleEffect.Stop();
		}
	}

	public void OpenBox() {
		CustomLogger.Log("OpenBOx 들어옴");

		animator.SetTrigger("Open");
		CustomLogger.Log("open 통과함");
		Invoke("ShowItemInfo", 0.65f);
	}

	public void Particle() {
		if (sparkleEffect != null) {
			sparkleEffect.Play();
		}
	}

	public void SetParticleColor(ItemRarity rarity) {
		if (sparkleEffect != null) {
			var mainModule = sparkleEffect.main;

			switch (rarity) {
				case ItemRarity.Normal:
					mainModule.startColor = Color.white;

					break;
				case ItemRarity.Rare:
					mainModule.startColor = Color.blue;

					break;
				case ItemRarity.Unique:
					mainModule.startColor = Color.red;

					break;
				default:
					mainModule.startColor = Color.white;

					break;
			}
		}
	}

	public void ShowItemInfo() {
		if (Gacha.Instance.inventoryUI != null && !Gacha.Instance.inventoryUI.CanAdditems(1)) {
			CustomLogger.LogWarning("인벤토리에 충분한 공간이 없습니다.");

			return;
		}

		if (isMultiGacha) {
			gacha.OnMultiGachaButtonClicked(gachaCount);
		} else {
			ItemInstance gachaItem = gacha.OnGachaButtonClicked();

			if (gachaItem != null) {
				SetParticleColor(gachaItem.itemData.rarity);
			}
		}

		EnableUIElements();
	}

	void EnableUIElements() {
		if (isMultiGacha) {
			if (Gacha.Instance.resultText != null) {
				Gacha.Instance.resultText.enabled = false;
			}

			if (Gacha.Instance.resultImage != null) {
				Gacha.Instance.resultImage.enabled = false;
			}
		} else {
			Transform multiGachaParent;

			if (Gacha.Instance.inventoryUI != null && !Gacha.Instance.inventoryUI.CanAdditems(1)) {
				if (Gacha.Instance.resultImage != null) {
					Gacha.Instance.resultImage.enabled = false;

					CustomLogger.Log("Gacha.Instance.resultImage.enabled : " + Gacha.Instance.resultImage.enabled);
				}

				if (Gacha.Instance.resultText != null) {
					Gacha.Instance.resultText.enabled = false;

					CustomLogger.Log("Gacha.Instance.resultText.enabled : " + Gacha.Instance.resultText.enabled);
				}

				if (Gacha.Instance.multiGachaResultText != null) {
					Gacha.Instance.multiGachaResultText.enabled = false;

					CustomLogger.Log("Gacha.Instance.multiGachaResultText.enabled : " +
					                 Gacha.Instance.multiGachaResultText.enabled);
				}

				multiGachaParent = Gacha.Instance.multiGachaResultText.transform.parent;

				foreach (Transform child in multiGachaParent) {
					if (child.name.StartsWith("GachaItem")) {
						child.gameObject.SetActive(false);
					}
				}

				return;
			}

			if (Gacha.Instance.resultImage != null) {
				Gacha.Instance.resultImage.enabled = true;
			}

			if (Gacha.Instance.resultText != null) {
				Gacha.Instance.resultText.enabled = true;
			}

			if (Gacha.Instance.multiGachaResultText != null) {
				Gacha.Instance.multiGachaResultText.enabled = false;
			}

			multiGachaParent = Gacha.Instance.multiGachaResultText.transform.parent;

			foreach (Transform child in multiGachaParent) {
				if (child.name.StartsWith("GachaItem")) {
					child.gameObject.SetActive(false);
				}
			}
		}
	}

	public void SetMultiGacha(bool isMulti, int count) {
		isMultiGacha = isMulti;
		gachaCount = count;
	}

	public void SingleGacha() {
		CustomLogger.Log("SingleGacha 눌림");

		if (Gacha.Instance.inventoryUI != null && Gacha.Instance.inventoryUI.CanAdditems(1)) {
			CustomLogger.Log("if 들어옴");
			SetMultiGacha(false, 1);
			OpenBox();
		} else {
			CustomLogger.LogWarning("인벤토리에 충분한 공간이 없습니다.");
		}

		CustomLogger.Log("if 통과함");
	}

	public void MultiGacha(int count) {
		CustomLogger.Log("MultiGacha 눌림");

		if (Gacha.Instance.inventoryUI != null && Gacha.Instance.inventoryUI.CanAdditems(count)) {
			SetMultiGacha(true, count);
			OpenBox();
		} else {
			CustomLogger.LogWarning("인벤토리에 충분한 공간이 없습니다.");
		}
	}
}