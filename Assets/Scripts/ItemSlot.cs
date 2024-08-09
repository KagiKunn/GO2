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


    private void Start()
    {
        if (item != null)
        {
            itemImage.sprite = item.icon;
        }

        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(canvas.transform, true);
        itemImage.raycastTarget = false;
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemImage.raycastTarget = true;

        RaycastResult result = eventData.pointerCurrentRaycast;
        if (result.gameObject != null)
        {
            ItemSlot targetSlot = result.gameObject.GetComponent<ItemSlot>();
            if (targetSlot != null && targetSlot != this)
            {
                InventoryUI.Instance.SwapItems(this, targetSlot);
            }
        }
        
        transform.SetParent(originalParent, true);
        transform.localPosition = Vector3.zero;
    }

    private void SwapItems(ItemSlot targetSlot)
    {
        ItemSO tempItem = targetSlot.item;
        targetSlot.item = this.item;
        this.item = tempItem;
        
        targetSlot.UpdateSlotUI();
        this.UpdateSlotUI();
    }

    public void UpdateSlotUI()
    {
        if (item != null)
        {
            itemImage.sprite = item.icon;
        }
        else
        {
            itemImage.sprite = null;
        }
    }
}
