using System.Collections;
using TMPro;
using UnityEngine;

public class SimplePopup : MonoBehaviour
{
    public GameObject popupBox;
    public TMP_Text popupText;

    private WaitForSeconds displayDuration = new WaitForSeconds(0.8f);

    private void Awake()
    {
        popupBox.SetActive(false);
    }

    public void ShowPopup(string message)
    {
        // 게임 오브젝트가 비활성화된 상태에서 활성화
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }

        popupText.text = message;
        StartCoroutine(DisplayPopup());
    }

    private IEnumerator DisplayPopup()
    {
        popupBox.SetActive(true);

        yield return displayDuration;

        popupBox.SetActive(false);

        // 팝업을 비활성화
        gameObject.SetActive(false);
    }
}