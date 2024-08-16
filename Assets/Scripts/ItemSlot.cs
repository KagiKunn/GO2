using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemInstance ItemInstance;
    public ItemSO item => ItemInstance?.itemData;
    public Image itemImage;

    private Transform originalParent;
    private Canvas canvas;
    private RectTransform rectTransform;
    private LayoutGroup layoutGroup;
    private int originalSiblingIndex;

    private InventoryUI inventoryUI;


    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        itemImage = GetComponentInChildren<Image>();

        inventoryUI = FindObjectOfType<InventoryUI>();
        
            if (inventoryUI != null)
            {
               inventoryUI.UpdateSlotUI(this.transform, ItemInstance);
               CustomLogger.Log($"Slot {gameObject.name} initialized with item: {item.itemName}");
            }
            else
            {
                CustomLogger.Log($"Slot {gameObject.name} has no item assigned.");
            }
        

        if (GetComponentInParent<Canvas>() == null)
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
        }
        
        ResetSlotPosition();
        
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
        if (this.ItemInstance == null || targetSlot.ItemInstance == null)
        {
            CustomLogger.Log($"Attempted to swap with a null item. This item: {this.ItemInstance?.itemData.itemName}, Target item: {targetSlot.ItemInstance?.itemData.itemName}");
            return;
        }
        
        CustomLogger.Log($"Before Swap: This item: {this.ItemInstance?.itemData.itemName}, Target item: {targetSlot.ItemInstance?.itemData.itemName}");
        
        ItemInstance tempInstance = targetSlot.ItemInstance;
        targetSlot.ItemInstance = this.ItemInstance;
        this.ItemInstance = tempInstance;
        
        CustomLogger.Log($"After Swap: This item: {this.ItemInstance?.itemData.itemName}, Target item: {targetSlot.ItemInstance?.itemData.itemName}");

        if (inventoryUI != null)
        {
            inventoryUI.UpdateSlotUI(this.transform, this.ItemInstance);
            inventoryUI.UpdateSlotUI(targetSlot.transform, targetSlot.ItemInstance);
        }
        else
        {
            CustomLogger.Log("InventoryUI instance not found.");
        }

        targetSlot.rectTransform.anchoredPosition = Vector3.zero;
        rectTransform.anchoredPosition = Vector3.zero;
        
        CustomLogger.Log($"Swapped items: {this.item?.itemName} with {targetSlot.item?.itemName}");
    }
    
    private void ResetSlotPosition()
    {
        transform.SetParent(originalParent, false);
        transform.SetSiblingIndex(originalSiblingIndex);
        rectTransform.anchoredPosition = Vector2.zero;
    }
}