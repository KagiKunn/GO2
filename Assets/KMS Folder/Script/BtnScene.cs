using System.Collections.Generic;

using TMPro;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BtnScene : MonoBehaviour {
	public GameObject defensePopupPrefab; // 팝업 프리팹을 연결할 수 있도록 public 변수로 선언
	public GameObject offencePopupPrefab;
	private bool unitSet = false;

	public void heroSceneChange() {
		SfxManager.Instance.clickSound();
		SceneManager.LoadScene("HeroManagement");
		CustomLogger.Log("Change HeroManagementScene successfuly!");
	}

	public void internalSceneChange() {
		PlayerLocalManager.Instance.Save();
		SfxManager.Instance.clickSound();
		SceneManager.LoadScene("InternalAffairs");
		CustomLogger.Log("Change InternalAffairsScene successfuly!");
	}

	public void DefenseSceneChange() {
		SfxManager.Instance.clickSound();
		if (GameObject.FindWithTag("Popup") != null) return;

		List<Triple<int, int, string>> allylist = PlayerLocalManager.Instance.lUnitList;

		foreach (Triple<int, int, string> ally in allylist) {
			if (ally.Item3 != "Default" && ally.Item2>-1) {
				unitSet = true;

				break;
			}
		}

		CustomLogger.Log(unitSet, "red");
		// 팝업을 생성하고, 팝업 UI에 접근
		GameObject popup = Instantiate(defensePopupPrefab);

		popup.tag = "Popup";

		// PopupBackground 패널 하위의 ConfirmButton 찾기
		Transform confirmButtonTransform = popup.transform.Find("PopupBackground/ConfirmButton");
		Button confirmButton = confirmButtonTransform.GetComponent<Button>();

		// PopupBackground 패널 하위의 CancelButton 찾기
		Transform cancelButtonTransform = popup.transform.Find("PopupBackground/CancelButton");
		Button cancelButton = cancelButtonTransform.GetComponent<Button>();

		// PopupBackground 패널 하위의 DefensePrefab 찾기
		Transform defensePopupTextTransform = popup.transform.Find("PopupBackground/DefensePopupText");

		// TextMeshProUGUI 컴포넌트 가져오기
		TextMeshProUGUI defensePopupText = defensePopupTextTransform.GetComponent<TextMeshProUGUI>();

		Transform noUnitButtonTransform = popup.transform.Find("PopupBackground/NoUnitButton");
		Button noUnitButton = noUnitButtonTransform.GetComponent<Button>();

		if (unitSet) {
			// 확인 버튼 클릭 이벤트 등록
			confirmButton.onClick.AddListener(() => {
				CustomLogger.Log("Confirm button clicked!");
				SceneManager.LoadScene("Defense");
				CustomLogger.Log("Change Defense successfuly!");
				Destroy(popup); // 팝업을 닫음
			});

			// 취소 버튼 클릭 시 팝업을 닫기
			cancelButton.onClick.AddListener(() => {
				CustomLogger.Log("Cancel button clicked!");
				Destroy(popup);
			});
		} else {
			LocalizedString localizedString = new LocalizedString
				{ TableReference = "UI", TableEntryReference = "ChooseUnit" };

			localizedString.StringChanged += (localizedText) => {
				defensePopupText.text = $"{localizedText}";
			};

			confirmButtonTransform.gameObject.SetActive(false);
			cancelButtonTransform.gameObject.SetActive(false);
			noUnitButtonTransform.gameObject.SetActive(true);

			noUnitButton.onClick.AddListener(() => {
				Destroy(popup);
			});
		}

		CustomLogger.Log("Event listeners added.");
	}

	public void OffenceSceneChange() {
		SfxManager.Instance.clickSound();
		if (GameObject.FindWithTag("Popup") != null) return;

		HeroList[] heroList = PlayerLocalManager.Instance.lHeroeList;

		int cnt = 0;

		for (int i = 0; i < heroList.Length; i++) {
			if (PlayerLocalManager.Instance.lHeroeList[i].Item3 > 0)
				cnt++;
		}

		// 팝업을 생성하고, 팝업 UI에 접근
		GameObject popup = Instantiate(offencePopupPrefab);

		popup.tag = "Popup";

		// PopupBackground 패널 하위의 ConfirmButton 찾기
		Transform confirmButtonTransform = popup.transform.Find("PopupBackground/ConfirmButton");
		Button confirmButton = confirmButtonTransform.GetComponent<Button>();

		// PopupBackground 패널 하위의 CancelButton 찾기
		Transform cancelButtonTransform = popup.transform.Find("PopupBackground/CancelButton");
		Button cancelButton = cancelButtonTransform.GetComponent<Button>();

		// PopupBackground 패널 하위의 OffencePopupText 찾기
		Transform offencePopupTextTransform = popup.transform.Find("PopupBackground/OffencePopupText");

		// TextMeshProUGUI 컴포넌트 가져오기
		TextMeshProUGUI offencePopupText = offencePopupTextTransform.GetComponent<TextMeshProUGUI>();

		Transform noHeroButtonTransform = popup.transform.Find("PopupBackground/NoHeroButton");
		Button noHeroButton = noHeroButtonTransform.GetComponent<Button>();

		if (PlayerLocalManager.Instance.lNextEnemy) {
			offencePopupText.text = "이미 클리어 하셨습니다.";

			confirmButtonTransform.gameObject.SetActive(false);
			cancelButtonTransform.gameObject.SetActive(false);
			noHeroButtonTransform.gameObject.SetActive(true);

			noHeroButton.onClick.AddListener(() => {
				Destroy(popup);
			});

			return;
		}

		if (cnt == 3) {
			// 확인 버튼 클릭 이벤트 등록
			confirmButton.onClick.AddListener(() => {
				CustomLogger.Log("Confirm button clicked!");

				//offence 출격 시 골드200 소모
				if (PlayerLocalManager.Instance.lMoney >= 200) {
					PlayerLocalManager.Instance.lMoney -= 200;
					PlayerLocalManager.Instance.Save();
					SceneManager.LoadScene("Offence");
					Destroy(popup); // 팝업을 닫음
				} else {
					// 팝업 텍스트를 수정
					offencePopupText.text = "NOT ENOUGH GOLD";
				}
			});

			// 취소 버튼 클릭 시 팝업을 닫기
			cancelButton.onClick.AddListener(() => {
				CustomLogger.Log("Cancel button clicked!");
				Destroy(popup);
			});
		} else {
			LocalizedString localizedString = new LocalizedString
				{ TableReference = "UI", TableEntryReference = "Choose3" };

			localizedString.StringChanged += (localizedText) => {
				offencePopupText.text = $"{localizedText}";
			};

			confirmButtonTransform.gameObject.SetActive(false);
			cancelButtonTransform.gameObject.SetActive(false);
			noHeroButtonTransform.gameObject.SetActive(true);

			noHeroButton.onClick.AddListener(() => {
				Destroy(popup);
			});
		}
	}

	public void UnitShopSceneChange() {
		SfxManager.Instance.clickSound();
		SceneManager.LoadScene("UnitShop");
		CustomLogger.Log("Change UnitShop successfuly!");
	}

	public void UnitManageSceneChange() {
		SfxManager.Instance.clickSound();
		SceneManager.LoadScene("UnitManagement");
		CustomLogger.Log("Change UnitManagement successfuly!");
	}
}