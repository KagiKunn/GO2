using System.Linq;
using TMPro;
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
    private GameObject draggingInstance;

    private bool isDragging = false;
    private bool isDraggable = true;
    public bool isDropped = false;
    
    private UnitGameManager unitGameManager;
    private Camera mainCamera;
    
    private void Awake()
    {
        Canvas = FindFirstObjectByType<Canvas>().transform;
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        unitGameManager = FindFirstObjectByType<UnitGameManager>();
        mainCamera = Camera.main;
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
            
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.6f;
        }
        
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Canvas.GetComponent<RectTransform>(), eventData.position, mainCamera, out localPoint);
            rect.localPosition = localPoint;
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