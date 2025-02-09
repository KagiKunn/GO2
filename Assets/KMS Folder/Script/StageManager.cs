using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour {
	public Button leftButton;
	public Button rightButton;
	public Button resetButton;

	public UnitGameManager unitGameManager;

	public GameObject[] slots;

	void Start() {
		leftButton.onClick.AddListener(ShowLeftStage);
		rightButton.onClick.AddListener(ShowRightStage);
		resetButton.onClick.AddListener(reset);

		ShowLeftStage();
	}

	private void ShowLeftStage() {
		SetSlotVisibility(0, 13);
		SetSlotInvisibility(14, 27);

		PlayerLocalManager.Instance.lUnitList = unitGameManager.userUnits;
		PlayerLocalManager.Instance.Save();;
		leftButton.gameObject.SetActive(false);
		rightButton.gameObject.SetActive(true);
	}

	private void ShowRightStage() {
		SetSlotVisibility(14, 27);
		SetSlotInvisibility(0, 13);


        PlayerLocalManager.Instance.lUnitList = unitGameManager.userUnits;
        PlayerLocalManager.Instance.Save();;
		rightButton.gameObject.SetActive(false);
		leftButton.gameObject.SetActive(true);
	}

	private void SetSlotVisibility(int start, int end) {
		for (int i = start; i <= end; i++) {
			slots[i].SetActive(true);
		}
	}

	private void SetSlotInvisibility(int start, int end) {
		for (int i = start; i <= end; i++) {
			slots[i].SetActive(false);
		}
	}

	private void reset() {
		unitGameManager.resetUnitsList();
	}

	private void OnDestroy() {
		
	}
}