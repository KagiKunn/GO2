using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingCredit : MonoBehaviour
{
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
    }

    private void Update()
    {
        // rectTransform의 anchoredPosition.y가 특정 값에 도달했을 때 씬을 전환합니다.
        if (rectTransform.anchoredPosition.y >= 2000f)
        {
            SceneManager.LoadScene("Title");
        }

        // 위로 움직이도록 anchoredPosition을 증가시킵니다.
        Vector2 newPosition = rectTransform.anchoredPosition;
        newPosition.y += 100f * Time.deltaTime; // y 값을 증가시킵니다.
        rectTransform.anchoredPosition = newPosition;
    }
}