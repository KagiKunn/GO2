using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitDropable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler {
	private Image image;
	private RectTransform rect;

	private void Awake() {
		image = GetComponent<Image>();
		rect = GetComponent<RectTransform>();
	}

	// 마우스 포인터가 현재 아이템 슬롯 영역 내부로 들어갈 때 1회 호출
	public void OnPointerEnter(PointerEventData eventData) {
		// 아이템 슬롯의 색상을 노란색으로 변경
		image.color = Color.yellow;
	}

	// 마우스 포인터가 현재 아이템 슬롯 영역을 빠져나갈 때 1회 호출
	public void OnPointerExit(PointerEventData eventData) {
		// 아이템 슬롯의 색상을 하얀색으로 변경
		image.color = Color.white;
	}

	public void OnDrop(PointerEventData eventData) {
		UnitDraggable draggedUnit = eventData.pointerDrag.GetComponent<UnitDraggable>();

		if (draggedUnit != null) {
			// 유닛 목록 내에서 해당 슬롯의 인덱스와 드래그된 유닛의 인덱스가 동일한 경우에만 드롭 허용
			if (transform.parent.name == "Unit" && transform.GetSiblingIndex() == draggedUnit.unitIndex) {
				// 드래그된 유닛의 부모를 현재 슬롯으로 설정하고 위치를 맞춥니다.
				draggedUnit.transform.SetParent(transform);
				draggedUnit.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
			}
			// 드롭존에서는 어떤 위치에도 드롭 가능
			else if (transform.parent.name == "UnitDrop") {
				// 드래그된 유닛의 부모를 현재 드롭존으로 설정하고 위치를 맞춥니다.
				draggedUnit.transform.SetParent(transform);
				draggedUnit.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
			}
		}
	}
}