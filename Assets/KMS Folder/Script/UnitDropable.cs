using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitDropable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    private Image image;
    private RectTransform rect;

    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }
    
    // 마우스 포인터가 현재 아이템 슬롯 영역 내부로 들어갈 때 1회 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 아이템 슬롯의 색상을 노란색으로 변경
        image.color = Color.yellow;
    }
    
    // 마우스 포인터가 현재 아이템 슬롯 영역을 빠져나갈 때 1회 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        // 아이템 슬롯의 색상을 하얀색으로 변경
        image.color = Color.white;
    }
    
    // 현재 아이템 슬롯 영역 내부에서 드롭했을 때 1회 호출
    public void OnDrop(PointerEventData eventData)
    {
        // pointerDrag는 현재 드래그 하고 있는 대상(=아이템)
        if (eventData.pointerDrag != null)
        {
            // 드래그하고 있는 대상의 부모를 현재 오브젝트(슬롯)로 설정하고, 위치를 현재 오브젝트 위치와 동일하게 설정
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.GetComponent<RectTransform>().position = rect.position;
        }
    }
}