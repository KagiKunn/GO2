using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingCredit : MonoBehaviour
{
    private RectTransform rectTransform;

    void Awake()
    {
        Time.timeScale = 1f;
        rectTransform = gameObject.GetComponent<RectTransform>();
        Debug.Log("Starting EndingCredit script");
    }

    private void Update()
    {
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform is not assigned!");
            return;
        }

        Debug.Log($"Current Position: {rectTransform.anchoredPosition.y}");

        if (rectTransform.anchoredPosition.y >= 2000f)
        {
            Debug.Log("Position reached 2000, loading Title scene.");
            SceneManager.LoadScene("Title");
        }

        Vector2 newPosition = rectTransform.anchoredPosition;
        newPosition.y += 100f * Time.deltaTime;
        rectTransform.anchoredPosition = newPosition;
    }
    void OnEnable()
    {
        Debug.Log("EndingCredit script enabled");
    }

    void OnDisable()
    {
        Debug.Log("EndingCredit script disabled");
    }
 
}