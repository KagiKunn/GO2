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
    private RectTransform draggingRect;

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
            previousParent = transform.parent;
            
            Transform unitTransform = previousParent.Find("Unit");
            Image unitImage = unitTransform.GetComponent<Image>();
            GameObject prefabname = unitGameManager.FindPrefabByName(unitName);
            
            draggingInstance = Instantiate(prefabname, Canvas);
            draggingRect = draggingInstance.GetComponent<RectTransform>();

            CanvasGroup prefabCanvasGroup = draggingInstance.GetComponent<CanvasGroup>();
            
            if (prefabCanvasGroup == null)
            {
                prefabCanvasGroup = draggingInstance.AddComponent<CanvasGroup>();
            }
            
            var newDraggable = draggingInstance.AddComponent<UnitDraggable>();
            newDraggable.unitName = unitName;
            newDraggable.Canvas = Canvas;
            newDraggable.previousParent = previousParent;
            newDraggable.mainCamera = mainCamera;
            newDraggable.unitGameManager = unitGameManager;
            newDraggable.SetDraggable(true);

            SetDraggingPosition(eventData);
            
            draggingRect.sizeDelta = new Vector2(200, 200);
            draggingRect.localScale = new Vector3(200, 200, 1);
            
            rect.gameObject.SetActive(true);

            draggingInstance.transform.SetAsLastSibling();
            draggingInstance.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        
    }
    
    private void SetDraggingPosition(PointerEventData eventData)
    {
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            Canvas.GetComponent<RectTransform>(), 
            eventData.position, 
            mainCamera, 
            out worldPoint
        );

        draggingRect.position = worldPoint;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        SetDraggingPosition(eventData);
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(draggingInstance);
        
        if (transform.parent == Canvas)
        {
            transform.SetParent(previousParent);
            rect.position = previousParent.GetComponent<RectTransform>().position;
        }
        canvasGroup.blocksRaycasts = true;
    }
}