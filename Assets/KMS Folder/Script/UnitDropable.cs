using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitDropable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    private Image image;
    private RectTransform rect;
    public UnitGameManager unitGameManager;

    private GameObject[] Prefabs;

    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        Prefabs = Resources.LoadAll<GameObject>("Defense/Unit");
        unitGameManager = FindFirstObjectByType<UnitGameManager>();
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
                image.color = Color.yellow;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            image.color = new Color(0.643f, 0.643f, 0.643f);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        UnitDraggable draggedUnit = eventData.pointerDrag.GetComponent<UnitDraggable>();

        if (draggedUnit != null)
        {
            int totalSiblings = transform.parent.childCount; // 부모 오브젝트의 전체 자식 수
            int slotIndex = totalSiblings - 1 - transform.GetSiblingIndex();

            TextMeshProUGUI unitText = draggedUnit.GetComponentInChildren<TextMeshProUGUI>();
            string unitName = unitText != null ? unitText.text : string.Empty;

            if (!string.IsNullOrEmpty(unitName))
            {
                int existingIndex = unitGameManager.selectedUnits
                    .FindIndex(x => x.Key == slotIndex && x.Value == "Default");

                if (existingIndex != -1)
                {
                    unitGameManager.selectedUnits[existingIndex] = new KeyValuePair<int, string>(slotIndex, unitName);
                }
                else
                {
                    unitGameManager.selectedUnits.Add(new KeyValuePair<int, string>(slotIndex, unitName));
                }

                CustomLogger.Log(slotIndex + " " + unitName);

                PlayerLocalManager.Instance.lAllyUnitList = unitGameManager.selectedUnits;
                PlayerLocalManager.Instance.Save();

                unitGameManager.RemoveUnitFromList(unitName);
                
                draggedUnit.transform.SetParent(transform);
                draggedUnit.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
                
                Image dropZoneImage = GetComponent<Image>();
                
                if (dropZoneImage != null)
                {
                    dropZoneImage.color = new Color(0.643f, 0.643f, 0.643f);
                    dropZoneImage.GetComponent<UnitDropable>().enabled = false; // 중복 배치 방지
                }
                
                draggedUnit.transform.SetParent(draggedUnit.previousParent);
                draggedUnit.GetComponent<RectTransform>().position = 
                    draggedUnit.previousParent.GetComponent<RectTransform>().position;
                
                unitGameManager.DisplayPrefab();
                
                draggedUnit.SetDraggable(false);
                draggedUnit.isDropped = true;
            }
        }
    }

    public void RemoveExistingPrefab(int slotIndex)
    {
        Transform slotTransform = unitGameManager.Slot[slotIndex].transform;

        if (slotTransform.childCount > 0)
        {
            foreach (Transform child in slotTransform)
            {
                if (child.name.EndsWith("(Clone)"))
                {
                    Destroy(child.gameObject);
                }
            }
        }
        
    }
}