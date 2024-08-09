using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemSO item;
    public Image itemImage;

    private Transform originalParent;
    private Canvas canvas;
    private RectTransform rectTransform;
    private LayoutGroup layoutGroup;
    private int originalSiblingIndex;


    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        itemImage = GetComponentInChildren<Image>();
        
        if (item != null && itemImage != null)
        {
            itemImage.sprite = item.icon;
            CustomLogger.Log($"ItemSlot initialized with item: {item.itemName}");
        }
        else
        {
            CustomLogger.Log("ItemSlot initialized without item or image.");
        }

        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            CustomLogger.Log("Canvas not found in parent hierarchy.");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        layoutGroup = originalParent.GetComponent<LayoutGroup>();
        originalSiblingIndex = transform.GetSiblingIndex();
        if (layoutGroup != null)
        {
            layoutGroup.enabled = false;
        }

        itemImage.raycastTarget = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemImage.raycastTarget = true;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        bool itemSwapped = false;
        foreach (var result in results)
        {
            ItemSlot targetSlot = result.gameObject.GetComponentInParent<ItemSlot>();
            
            if (targetSlot != null && targetSlot != this)
            {
                CustomLogger.Log($"RayCast hit: {targetSlot.name}");
                SwapItems(targetSlot);
                itemSwapped = true;
                break;
            }
            else
            {
                CustomLogger.Log($"RayCast hit a non-slot object: {result.gameObject.name}");
            }
        }
        
        transform.SetParent(originalParent, false);
        transform.SetSiblingIndex(originalSiblingIndex);
        rectTransform.localPosition = Vector3.zero;
        
        if (layoutGroup != null)
        {
            layoutGroup.enabled = true;
            LayoutRebuilder.ForceRebuildLayoutImmediate(originalParent.GetComponent<RectTransform>());
        }

        if (!itemSwapped)
        {
            CustomLogger.Log("아이템교환 실패");
        }
        
        CustomLogger.Log($"OnEndDrag: Parent = {transform.parent.name}, Position = {rectTransform.localPosition}");
    }

    private void SwapItems(ItemSlot targetSlot)
    {
        if (this.item == null || targetSlot.item == null)
        {
            CustomLogger.Log("Attempted to swap with a null item.");
            return;
        }
        
        CustomLogger.Log($"Before Swap: This item: {this.item?.itemName}, Target item: {targetSlot.item?.itemName}");
        
        ItemSO tempItem = targetSlot.item;
        targetSlot.item = this.item;
        this.item = tempItem;

        targetSlot.UpdateSlotUI();
        this.UpdateSlotUI();

        // targetSlot.rectTransform.anchoredPosition = Vector2.zero;
        // rectTransform.anchoredPosition = Vector2.zero;
        
        CustomLogger.Log($"Swapped items: {this.item?.itemName} with {targetSlot.item?.itemName}");
    }

    public void UpdateSlotUI()
    {
        if (item != null && itemImage != null)
        {
            itemImage.sprite = item.icon;
        }
        else
        {
            itemImage.sprite = null;
        }
    }
}