using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitDropable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    private Image image;
    private RectTransform rect;

    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null) // pointerDrag가 null인지 확인
        {
            UnitDraggable draggedUnit = eventData.pointerDrag.GetComponent<UnitDraggable>();
            if (draggedUnit != null && !draggedUnit.isDropped) // draggedUnit이 null이 아닌지 확인
            {
                image.color = Color.yellow; // 배치되지 않은 유닛만 이벤트 발동 하도록
            }
        }
    }
    
    // 마우스 포인터가 현재 아이템 슬롯 영역을 빠져나갈 때 1회 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null) // pointerDrag가 null인지 확인
        {
            UnitDraggable draggedUnit = eventData.pointerDrag.GetComponent<UnitDraggable>();
            if (draggedUnit != null && !draggedUnit.isDropped) // draggedUnit이 null이 아닌지 확인
            {
                // 아이템 슬롯의 색상을 하얀색으로 변경
                image.color = Color.white;
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        UnitDraggable draggedUnit = eventData.pointerDrag.GetComponent<UnitDraggable>();

        if (draggedUnit != null)
        {
            // 드래그된 유닛을 현재 드롭존으로 이동
            draggedUnit.transform.SetParent(transform);
            draggedUnit.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
            
            // 드래그된 유닛의 이미지를 드롭존에 표시
            Image dropZoneImage = GetComponent<Image>();
            if (dropZoneImage != null)
            {
                dropZoneImage.sprite = draggedUnit.unitData.UnitImage; // 유닛 데이터에 연결된 이미지 설정
                dropZoneImage.color = Color.white; // 색상을 기본으로 설정
            }
            // 유닛 목록의 원본 이미지를 어둡게 표시하여 배치 완료를 시각적으로 표시
            Image originalImage = draggedUnit.previousParent.GetComponent<Image>();
            if (originalImage != null)
            { 
                originalImage.color = new Color(0.5f, 0.5f, 0.5f, 1.0f); // 어둡게
            }
            // 드래그된 유닛의 부모를 다시 목록으로 설정하고, 드롭존에는 데이터에서 추출한 이미지만 표시
            draggedUnit.transform.SetParent(draggedUnit.previousParent);
            draggedUnit.GetComponent<RectTransform>().position = draggedUnit.previousParent.GetComponent<RectTransform>().position;
                
            // 드래그된 유닛의 슬롯 이벤트를 비활성화하여 중복 배치를 방지
            UnitDraggable originalDraggable = draggedUnit.previousParent.GetComponentInChildren<UnitDraggable>();
            if (originalDraggable != null)
            { 
                originalDraggable.enabled = false;
            }
            draggedUnit.isDropped = true;
        }
    }
}
