using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 유닛 목록에 유저가 보유중인 유닛을 표시하기 위한 스크립트
public class UnitSlotManager : MonoBehaviour
{
    public Transform contentParent; // UnitSlot들을 포함하는 부모 오브젝트
    private List<UnitData> userUnits; // 유저가 보유한 모든 유닛 데이터 목록
    private List<Image> unitImages = new List<Image>(); // 각 슬롯의 하위에 있는 유닛 이미지 컴포넌트 목록
    private List<Graphic> unitSlots = new List<Graphic>(); // 유닛 슬롯(Dropable) 
    private List<UnitDraggable> unitDraggables = new List<UnitDraggable>(); // 각 슬롯의 하위에 있는 UnitDraggable 컴포넌트 목록

    private void Awake()
    {
        // 슬롯의 하위 이미지 컴포넌트를 미리 찾아서 저장
        for (int i = 0; i < contentParent.childCount; i++)
        {
            Transform slot = contentParent.GetChild(i);
            Image unitImage = slot.Find("Unit").GetComponent<Image>();
            UnitDraggable unitDraggable = slot.Find("Unit").GetComponent<UnitDraggable>();
            Graphic slotGraphic = slot.GetComponent<Graphic>();

            if (unitImage != null && unitDraggable != null && slotGraphic != null)
            {
                unitImages.Add(unitImage);
                unitDraggables.Add(unitDraggable);
                unitSlots.Add(slotGraphic);
            }
        }
    }
    
    private void Start()
    {
        // 유저가 보유한 모든 유닛 데이터를 가져와 슬롯에 배치
        userUnits = UnitGameManager.Instance.GetUnits();
        AssignUnitsToSlots();
        UpdateDraggableStates(); // UpdateDraggableStates를 Start 메서드에서 호출하도록 변경
    }

    private void AssignUnitsToSlots()
    {
        // 슬롯의 개수와 유저가 가진 유닛 개수 중 작은 값을 사용하여 순회
        int unitCount = Mathf.Min(unitImages.Count, userUnits.Count);

        // 각 슬롯의 하위 이미지에 유닛 데이터를 할당
        for (int i = 0; i < unitCount; i++)
        {
            SetUnitData(unitImages[i], unitDraggables[i], userUnits[i]);
        }

        // 유저가 가진 유닛보다 슬롯이 많을 경우, 나머지 이미지 비우고 이벤트 비활성화
        for (int i = unitCount; i < unitImages.Count; i++)
        {
            SetUnitData(unitImages[i], unitDraggables[i], null);
            unitSlots[i].raycastTarget = false;
        }
    }

    private void SetUnitData(Image unitImage, UnitDraggable unitDraggable, UnitData data)
    {
        if (data != null && data.UnitImage != null)
        {
            unitImage.sprite = data.UnitImage;
            unitImage.color = Color.white; // 이미지를 기본 색상으로 설정
            unitImage.enabled = true; // 이미지 표시

            unitDraggable.unitData = data; // UnitData를 UnitDraggable에 할당
            unitDraggable.unitIndex = unitImages.IndexOf(unitImage); // 인덱스 설정
            unitSlots[unitImages.IndexOf(unitImage)].raycastTarget = true; // 슬롯의 이벤트 활성화
        }
        else
        {
            unitImage.sprite = null;
            unitImage.enabled = false; // 이미지 숨기기
            unitDraggable.unitData = null; // UnitData 초기화
            unitSlots[unitImages.IndexOf(unitImage)].raycastTarget = false; // 슬롯의 이벤트 비활성화
        }
    }

    public void UpdateDraggableStates()
    {
        List<UnitData> selectedUnits = UnitGameManager.Instance.GetSelectedUnits();

        foreach (var draggable in unitDraggables)
        {
            if (selectedUnits.Contains(draggable.unitData))
            {
                draggable.SetDraggable(false); // 이미 배치된 유닛은 드래그 불가
                draggable.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1.0f); // 어둡게 표시
            }
            else
            {
                draggable.SetDraggable(true); // 배치되지 않은 유닛만 드래그 가능
                draggable.GetComponent<Image>().color = Color.white; // 기본 색상으로 표시
            }
        }
    }
    
    public void ResetUnitDrag()
    {
        foreach (var unitDraggable in unitDraggables)
        {
            if (unitDraggable != null)
            {
                unitDraggable.isDropped = false;  // 드래그 상태 초기화
                unitDraggable.enabled = true;  // 드래그 기능 다시 활성화
                unitDraggable.GetComponent<CanvasGroup>().blocksRaycasts = true;  // 드래그 가능하게 설정
                
                // 이미지를 초기 상태로 되돌림
                Image originalImage = unitDraggable.GetComponent<Image>();
                if (originalImage != null)
                {
                    originalImage.color = Color.white;  // 색상을 흰색으로 초기화
                }
                UpdateDraggableStates();
            }
        }
    }
}