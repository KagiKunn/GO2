using UnityEngine;
using UnityEngine.EventSystems;

public class AllyHoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Transform playerObjCircle;

    public void Initialize(Transform circle)
    {
        playerObjCircle = circle;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playerObjCircle != null)
        {
            Vector3 offset = new Vector3(0, -GetComponent<Collider2D>().bounds.extents.y, 0);
            playerObjCircle.position = transform.position + offset;
            playerObjCircle.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (playerObjCircle != null)
        {
            playerObjCircle.gameObject.SetActive(false);
        }
    }
}