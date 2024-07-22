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
      Invoke("RestoreOriginal",0.3f);
      CustomLogger.Log("Change color method", "blue");
   }

   private void RestoreOriginal()
   {
      targetImage.sprite = originalImage;
      btn_txt.color = originalTxtColor;
      CustomLogger.Log("Restore original Color", "green");
   }
}
