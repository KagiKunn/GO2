using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string unitName;
    
    private Transform Canvas;
    public Transform previousParent; 
    private RectTransform rect;
    private CanvasGroup canvasGroup;

    public int unitIndex;
    
    private bool isDragging = false;
    private bool isDraggable = true;
    public bool isDropped = false;
    
    private StageManager stageManager;
    
    private void Awake()
    {
        Canvas = FindFirstObjectByType<Canvas>().transform;
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        stageManager = FindFirstObjectByType<StageManager>();
    }
    
    public void SetDraggable(bool draggable)
    {
        isDraggable = draggable;
        canvasGroup.blocksRaycasts = draggable;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!eventData.pointerPressRaycast.gameObject.GetComponent<Button>())
        {
            isDragging = true;
            
            previousParent = transform.parent;
            
            transform.SetParent(Canvas);
            transform.SetAsLastSibling();
            
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }
        
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            rect.position = eventData.position;
        }

    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            isDragging = false;

            if (transform.parent == Canvas)
            {
                transform.SetParent(previousParent);
                rect.position = previousParent.GetComponent<RectTransform>().position;
            }
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.6f;
        }
    }
}