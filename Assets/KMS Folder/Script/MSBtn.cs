using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MSBtn : MonoBehaviour
{
    // 버튼 클릭 시 버튼 색상, 버튼 텍스트 색상 변경 스크립트

    public Image targetImage;
    public Sprite newImage;
    private Sprite originalImage;

    public TextMeshProUGUI btn_txt;
    public Color originalTxtColor;
    private Color newTxtColor;

    private void Start()
    {
        originalImage = targetImage.sprite;
        originalTxtColor = btn_txt.color;

        CustomLogger.Log("Original IMG Set", "red");
    }

    public void Change()
    {
        targetImage.sprite = newImage;
        newTxtColor = new Color(156f / 255f, 146f / 255f, 73f / 255f);
        btn_txt.color = newTxtColor;
        Invoke("RestoreOriginal", 0.3f);

        CustomLogger.Log("Change color method", Color.cyan);
    }

    private void RestoreOriginal()
    {
        targetImage.sprite = originalImage;
        btn_txt.color = originalTxtColor;

        CustomLogger.Log("Restore original Color");
    }

    public void CallRepairPopup()
    {
        if (Mathf.Approximately(PlayerLocalManager.Instance.lCastleHp, PlayerLocalManager.Instance.lCastleMaxHp))
        {
            GameObject fullPopup = Instantiate(Resources.Load<GameObject>("PreFab/SmithPopupHPfull"));
            Transform confirmButtonTransform = fullPopup.transform.Find("PopupBackground/ConfirmButton");
            Button confirmButton = confirmButtonTransform.GetComponent<Button>();
            
            confirmButton.onClick.AddListener(() =>
            {
                CustomLogger.Log("Confirm button clicked!");
                Destroy(fullPopup); // 팝업을 닫음
            });
        }
        else
        {
            Instantiate(Resources.Load<GameObject>("PreFab/SmithPopupCanvas"));
        }
    }
}