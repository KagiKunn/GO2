using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPlacementManager : MonoBehaviour
{
    public Transform contentParent;
    private List<SlotUnitData> placementUnits;
    private List<Image> placementImages = new List<Image>();
    public PlacementUnit placementUnit;

    private void Awake()
    {
        for (int i = 0; i < contentParent.childCount; i++)
        {
            Transform slot = contentParent.GetChild(i);
            Image placementImage = slot.GetComponent<Image>();

            if (placementImage != null)
            {
                placementImages.Add(placementImage);
            }
            
        }
            // 저장된 유닛의 배치 정보를 불러와서 배치 슬롯에 반영
            UnitGameManager.Instance.LoadUnitFormation();
            AssignSavedUnitsToSlots();
    }

    private void AssignSavedUnitsToSlots()
    {
        List<SlotUnitData> savedUnits = placementUnit.GetSlotUnitDataList();
        
        // savedUnits가 null이거나 비어있을 경우에 대비하여 안전하게 처리
        if (savedUnits == null || savedUnits.Count == 0)
        {
            Debug.LogWarning("No saved unit data available.");
            return;
        }

        // 저장된 유닛 배치 정보를 각 배치 슬롯에 할당
        foreach (var slotUnitData in savedUnits)
        {
            int slotIndex = slotUnitData.SlotIndex;
            if (slotIndex >= 0 && slotIndex < placementImages.Count)
            {
                if (slotUnitData.UnitData != null)
                {
                    SetUnitData(placementImages[slotIndex], slotUnitData.UnitData);  // 각 슬롯 인덱스에 맞춰 유닛을 배치
                }
                else
                {
                    SetUnitData(placementImages[slotIndex], null);  // 유닛 데이터가 없을 경우 슬롯을 초기화
                }
            }
            else
            {
                Debug.LogError($"Invalid Slot Index or Null Image Reference: {slotUnitData.SlotIndex}");
            }
        }
    }

    private void SetUnitData(Image unitImage, UnitData data)
    {
        if (data != null && data.UnitImage != null)
        {
            unitImage.sprite = data.UnitImage;
            unitImage.color = Color.white;  // 이미지를 기본 색상으로 설정
            unitImage.enabled = true;  // 이미지 표시
        }
    }
}
