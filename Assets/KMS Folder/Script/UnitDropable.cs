using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitDropable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    private Image image;
    private RectTransform rect;
    public UnitData assignedUnitData; // 슬롯에 배정된 유닛 데이터

    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            UnitDraggable draggedUnit = eventData.pointerDrag.GetComponent<UnitDraggable>();
            if (draggedUnit != null && !draggedUnit.isDropped)
            {
                image.color = Color.yellow; 
            }
        }
    }
    
    // 마우스 포인터가 현재 아이템 슬롯 영역을 빠져나갈 때 1회 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null) 
        {
            UnitDraggable draggedUnit = eventData.pointerDrag.GetComponent<UnitDraggable>();
            if (draggedUnit != null && !draggedUnit.isDropped) 
            {
                
                image.color = Color.white;
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        UnitDraggable draggedUnit = eventData.pointerDrag.GetComponent<UnitDraggable>();

        if (draggedUnit != null)
        {
            // 드래그된 유닛의 데이터를 현재 드롭존에 저장
            assignedUnitData = draggedUnit.unitData;
            
            // 드래그된 유닛을 현재 드롭존으로 이동
            draggedUnit.transform.SetParent(transform);
            draggedUnit.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
            
            Image dropZoneImage = GetComponent<Image>();
            if (dropZoneImage != null)
            {
                dropZoneImage.sprite = draggedUnit.unitData.UnitImage; 
                dropZoneImage.color = Color.white;
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
