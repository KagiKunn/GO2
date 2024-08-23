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
    
    private void Awake()
    {
        Canvas = FindFirstObjectByType<Canvas>().transform;
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        unitGameManager = FindFirstObjectByType<UnitGameManager>();
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
            Transform unitTransform = previousParent.Find("Unit");
            unitName = unitTransform.GetComponent<TextMeshProUGUI>().text;
            
            CustomLogger.Log(unitName, Color.magenta);
            
            GameObject prefabname = unitGameManager.FindPrefabByName(unitName);
            
            if(prefabname != null)
            {
                draggingInstance = Instantiate(prefabname, Canvas);
                RectTransform draggingRect = draggingInstance.GetComponent<RectTransform>();

                // 크기와 스케일 설정
                draggingRect.sizeDelta = new Vector2(200, 200); 
                draggingRect.localScale = new Vector3(1, 1, 1);
            }
            
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
            // rect.position = eventData.position;
            draggingInstance.GetComponent<RectTransform>().position = eventData.position;
        }

    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            isDragging = false;

            if (draggingInstance != null)
            {
                Destroy(draggingInstance);
            }
            
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