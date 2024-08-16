using UnityEngine;
using UnityEngine.UI;

public class NextStageButton : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        gameObject.SetActive(false);
    }
}