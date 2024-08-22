using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitDropable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    private Image image;
    private RectTransform rect;
    public UnitData assignedUnitData;

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
            // 현재 드롭된 위치의 인덱스 가져오기
            // int slotIndex = transform.GetSiblingIndex();
            //
            // // SaveUnitList 호출하여 유닛 정보 저장
            // SaveUnitList(draggedUnit.unitName, draggedUnit.unitPrefab, slotIndex);
            
            // assignedUnitData = draggedUnit.unitData;

            draggedUnit.transform.SetParent(transform);
            draggedUnit.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;

            Image dropZoneImage = GetComponent<Image>();
            
            if (dropZoneImage != null) // 배치 후
            {
                // dropZoneImage.sprite = draggedUnit.unitData.UnitImage;
                dropZoneImage.color = Color.white;
            }
            
            draggedUnit.transform.SetParent(draggedUnit.previousParent);
            draggedUnit.GetComponent<RectTransform>().position = draggedUnit.previousParent.GetComponent<RectTransform>().position;
            
            UnitDraggable originalDraggable = draggedUnit.previousParent.GetComponentInChildren<UnitDraggable>();
            if (originalDraggable != null)
            { 
                originalDraggable.enabled = false;
            }
            draggedUnit.isDropped = true;
        }
    }
}