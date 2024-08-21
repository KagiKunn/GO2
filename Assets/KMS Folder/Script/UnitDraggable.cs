using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class UnitDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{   
    private Transform Canvas;
    public Transform previousParent; 
    private RectTransform rect;
    private CanvasGroup canvasGroup;

    public int unitIndex;
    public UnitData unitData;
    
    private bool isDragging = false;
    private bool isDraggable = true;
    public bool isDropped = false;
    
    private UnitGameManagerA unitGameManager;
    private StageManager stageManager;
    
    private void Awake()
    {
        Canvas = FindFirstObjectByType<Canvas>().transform;
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        unitGameManager = UnitGameManagerA.Instance;
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
            
            canvasGroup.blocksRaycasts = false;
            canvasGroup.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        }
        
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            rect.position = eventData.position;
        }

    }
    // 현재 오브젝트의 드래그를 종료할 때 1회 호출
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
            canvasGroup.blocksRaycasts = true;
            canvasGroup.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        }
    }
}