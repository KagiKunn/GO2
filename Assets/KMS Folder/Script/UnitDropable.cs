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
    
    // 마우스 포인터가 현재 아이템 슬롯 영역 내부로 들어갈 때 1회 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = Color.yellow;
    }
    
    // 마우스 포인터가 현재 아이템 슬롯 영역을 빠져나갈 때 1회 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        // 아이템 슬롯의 색상을 하얀색으로 변경
        image.color = Color.white;
    }

    public void OnDrop(PointerEventData eventData)
    {
        UnitDraggable draggedUnit = eventData.pointerDrag.GetComponent<UnitDraggable>();

        if (draggedUnit != null)
        {
            // 드롭존에 유닛이 이미 존재하는지 확인(DropZone내에서 중복 배치를 없애기 위해)
            Transform existUnit = transform.childCount > 0 ? transform.GetChild(0) : null;
            
            // 드롭존 내의 유닛과 드래그된 유닛의 위치를 서로 교체
            if (existUnit != null)
            {
                existUnit.SetParent(draggedUnit.previousParent);
                existUnit.GetComponent<RectTransform>().position = draggedUnit.previousParent.GetComponent<RectTransform>().position;
            }

            // 드래그된 유닛을 현재 드롭존으로 이동
            draggedUnit.transform.SetParent(transform);
            draggedUnit.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
            
            // // 유닛 목록 내에서 해당 슬롯의 인덱스와 드래그된 유닛의 인덱스가 동일한 경우에만 드롭 허용
            // if (transform.parent.name == "Unit" && transform.GetSiblingIndex() == draggedUnit.unitIndex)
            // {
            //     // 드래그된 유닛의 부모를 현재 슬롯으로 설정하고 위치를 맞춥니다.
            //     draggedUnit.transform.SetParent(transform);
            //     draggedUnit.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
                
                // 유닛 목록의 원본 이미지를 어둡게 표시하여 배치 완료를 시각적으로 표시
                Image originalImage = draggedUnit.previousParent.GetComponent<Image>();
                
            // }
            // 드롭존에서는 어떤 위치에도 드롭 가능
            if (transform.parent.name == "UnitDrop")
            {
                if (existUnit != null)
                {
                    // 이미 드롭존에 유닛이 있는 경우, 기존 유닛과 드래그된 유닛의 위치를 서로 교체
                    existUnit.SetParent(draggedUnit.previousParent);
                    existUnit.GetComponent<RectTransform>().position = draggedUnit.previousParent.GetComponent<RectTransform>().position;
                }
                // 프리팹을 드롭존에 인스턴스화하여 표시
                Image dropZoneImage = GetComponent<Image>();
                if (dropZoneImage != null)
                {
                    dropZoneImage.sprite = draggedUnit.unitData.UnitImage; // 유닛 데이터에 연결된 이미지 설정
                    dropZoneImage.color = Color.white; // 색상을 기본으로 설정
                }
                // 드래그된 유닛의 부모를 다시 목록으로 설정하고, 드롭존에는 데이터에서 추출한 이미지만 표시
                draggedUnit.transform.SetParent(draggedUnit.previousParent);
                draggedUnit.GetComponent<RectTransform>().position = draggedUnit.previousParent.GetComponent<RectTransform>().position;
                originalImage = draggedUnit.previousParent.GetComponent<Image>();
            }
        }
    }
}