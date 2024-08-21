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
            assignedUnitData = draggedUnit.unitData;
            
            CustomLogger.Log(gameObject.transform.parent.name);
            
            // 드롭된 위치에 따라 배치 상태를 저장
            int placement = 0;
            if (gameObject.transform.parent.name.Contains("Left Wall Stage"))
            {
                placement = 1; // 왼쪽 성벽
            }
            else if (gameObject.transform.parent.name.Contains("Right Wall Stage"))
            {
                placement = 2; // 오른쪽 성벽
            }

            // UnitGameManagerA에 배치 상태를 저장
            UnitGameManagerA.Instance.SaveUnitPlacement(draggedUnit.unitData, placement);

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