using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BtnScene : MonoBehaviour
{
    public GameObject defensePopupPrefab; // 팝업 프리팹을 연결할 수 있도록 public 변수로 선언
    public GameObject offencePopupPrefab;

    public void heroSceneChange()
    {
        SceneManager.LoadScene("HeroManagement");
        CustomLogger.Log("Change HeroManagementScene successfuly!");
    }

    public void internalSceneChange()
    {
        SceneManager.LoadScene("InternalAffairs");
        CustomLogger.Log("Change InternalAffairsScene successfuly!");
    }

    public void DefenseSceneChange()
    {
        // 팝업을 생성하고, 팝업 UI에 접근
        GameObject popup = Instantiate(defensePopupPrefab);

        // PopupBackground 패널 하위의 ConfirmButton 찾기
        Transform confirmButtonTransform = popup.transform.Find("PopupBackground/ConfirmButton");
        Button confirmButton = confirmButtonTransform.GetComponent<Button>();

        // PopupBackground 패널 하위의 CancelButton 찾기
        Transform cancelButtonTransform = popup.transform.Find("PopupBackground/CancelButton");
        Button cancelButton = cancelButtonTransform.GetComponent<Button>();

        // 확인 버튼 클릭 이벤트 등록
        confirmButton.onClick.AddListener(() =>
        {
            CustomLogger.Log("Confirm button clicked!");
            SceneManager.LoadScene("Defense");
            CustomLogger.Log("Change Defense successfuly!");
            Destroy(popup); // 팝업을 닫음
        });

        // 취소 버튼 클릭 시 팝업을 닫기
        cancelButton.onClick.AddListener(() =>
        {
            CustomLogger.Log("Cancel button clicked!");
            Destroy(popup);
        });

        CustomLogger.Log("Event listeners added.");
    }

    public void OffenceSceneChange()
    {
        // 팝업을 생성하고, 팝업 UI에 접근
        GameObject popup = Instantiate(offencePopupPrefab);

        // PopupBackground 패널 하위의 ConfirmButton 찾기
        Transform confirmButtonTransform = popup.transform.Find("PopupBackground/ConfirmButton");
        Button confirmButton = confirmButtonTransform.GetComponent<Button>();

        // PopupBackground 패널 하위의 CancelButton 찾기
        Transform cancelButtonTransform = popup.transform.Find("PopupBackground/CancelButton");
        Button cancelButton = cancelButtonTransform.GetComponent<Button>();
        
        // PopupBackground 패널 하위의 OffencePopupText 찾기
        Transform offencePopupTextTransform = popup.transform.Find("PopupBackground/OffencePopupText");

        // TextMeshProUGUI 컴포넌트 가져오기
        TextMeshProUGUI offencePopupText = offencePopupTextTransform.GetComponent<TextMeshProUGUI>();

        // 확인 버튼 클릭 이벤트 등록
        confirmButton.onClick.AddListener(() =>
        {
            CustomLogger.Log("Confirm button clicked!");

            //offence 출격 시 골드200 소모
            if (PlayerLocalManager.Instance.lMoney >= 200)
            {
                PlayerLocalManager.Instance.lMoney -= 200;
                PlayerLocalManager.Instance.Save();
                SceneManager.LoadScene("Offence");
                Destroy(popup); // 팝업을 닫음
            }
            else
            {
                // 팝업 텍스트를 수정
                offencePopupText.text = "NOT ENOUGH GOLD";
            }
        });

        // 취소 버튼 클릭 시 팝업을 닫기
        cancelButton.onClick.AddListener(() =>
        {
            CustomLogger.Log("Cancel button clicked!");
            Destroy(popup);
        });
    }

    public void UnitManagementSceneChange()
    {
        SceneManager.LoadScene("UnitManagementRight");
        CustomLogger.Log("Change UnitManagement successfuly!");
    }

    public void UnitManagementLeftSceneChange()
    {
        SceneManager.LoadScene("UnitManagementLeft");
        CustomLogger.Log("Change UnitManagementLeft successfuly!");
    }

    public void UnitShopSceneChange()
    {
        SceneManager.LoadScene("UnitShop");
        CustomLogger.Log("Change UnitShop successfuly!");
    }
}