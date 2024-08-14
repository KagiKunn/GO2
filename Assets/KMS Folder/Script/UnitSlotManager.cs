using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections.Generic;

public class UnitSlotManager : MonoBehaviour {
	public Transform contentParent; // UnitSlot들을 포함하는 부모 오브젝트
	private List<UnitData> userUnits; // 유저가 보유한 모든 유닛 데이터 목록
	private List<Image> unitImages = new List<Image>(); // 각 슬롯의 하위 유닛 이미지 컴포넌트 목록

	private void Awake() {
		// 슬롯의 하위 이미지 컴포넌트를 미리 찾아서 저장
		for (int i = 0; i < contentParent.childCount; i++) {
			Transform slot = contentParent.GetChild(i);
			Image unitImage = slot.Find("Unit").GetComponent<Image>();

			if (unitImage != null) {
				unitImages.Add(unitImage);
			}
		}

		// 유저가 보유한 모든 유닛 데이터를 가져와 슬롯에 배치
		userUnits = UnitGameManager.Instance.GetUnits();
		AssignUnitsToSlots();
	}

	private void AssignUnitsToSlots() {
		// 슬롯의 개수와 유저가 가진 유닛 개수 중 작은 값을 사용하여 순회
		int unitCount = Mathf.Min(unitImages.Count, userUnits.Count);

		// 각 슬롯의 하위 이미지에 유닛 데이터를 할당
		for (int i = 0; i < unitCount; i++) {
			SetUnitData(unitImages[i], userUnits[i]);
		}

		// 유저가 가진 유닛보다 슬롯이 많을 경우, 나머지 이미지 비우고 이벤트 비활성화
		for (int i = unitCount; i < unitImages.Count; i++) {
			SetUnitData(unitImages[i], null); // 이미지 비우기 및 이벤트 비활성화
			DisableSlotEvents(unitImages[i].transform.parent.gameObject); // 슬롯의 이벤트 비활성화
		}
	}

	private void SetUnitData(Image unitImage, UnitData data) {
		if (data != null && data.UnitImage != null) {
			unitImage.sprite = data.UnitImage;
			unitImage.color = Color.white; // 이미지를 기본 색상으로 설정
			unitImage.enabled = true; // 이미지 표시

			EnableSlotEvents(unitImage.transform.parent.gameObject); // 슬롯의 이벤트 활성화
		} else {
			unitImage.sprite = null;
			unitImage.enabled = false; // 이미지 숨기기
			DisableSlotEvents(unitImage.transform.parent.gameObject); // 슬롯의 이벤트 비활성화
		}
	}

	private void EnableSlotEvents(GameObject slot) {
		// 슬롯의 이벤트를 활성화
		EventTrigger eventTrigger = slot.GetComponent<EventTrigger>();

		if (eventTrigger != null) {
			eventTrigger.enabled = true;
		}
	}

	private void DisableSlotEvents(GameObject slot) {
		// 슬롯의 이벤트를 비활성화
		EventTrigger eventTrigger = slot.GetComponent<EventTrigger>();

		if (eventTrigger != null) {
			eventTrigger.enabled = false;
		}
	}

	public UnitData GetUnitData(Image unitImage) {
		// 해당 이미지에 연결된 UnitData를 반환 (추가 로직 필요 시 구현)
		return null; // 기본적으로 null 반환, 필요시 구현
	}

	// 편성되었을때 유닛창의 유닛 이미지를 어둡게 함
	public void SetDropped(Image unitImage, bool dropped) {
		if (unitImage != null) {
			unitImage.color = dropped ? new Color(0.5f, 0.5f, 0.5f, 1.0f) : Color.white;
		}
	}
}