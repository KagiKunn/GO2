using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public Button leftButton;
    public Button rightButton;
    public Button resetButton;
    
    public UnitGameManager unitGameManager;
    
    public GameObject[] slots;

    void Start()
    {

        leftButton.onClick.AddListener(ShowLeftStage);
        rightButton.onClick.AddListener(ShowRightStage);
        resetButton.onClick.AddListener(reset);
        
        ShowLeftStage(); 

        CustomLogger.Log("StageManagement 스타트 메서드 활성화", "green");
    }
    

    private void ShowLeftStage()
    {
        CustomLogger.Log("show left stage start");
        SetSlotVisibility(0, 13);
        SetSlotInvisibility(14, 27);
        
        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(true);

    }

    private void ShowRightStage()
    {
        CustomLogger.Log("show right stage start");
        SetSlotVisibility(14, 27);
        SetSlotInvisibility(0, 13);

        rightButton.gameObject.SetActive(false);
        leftButton.gameObject.SetActive(true);
    }

    private void SetSlotVisibility(int start, int end)
    {
        for (int i = start; i <= end; i++)
        {
            CustomLogger.Log(slots[i].name);
            slots[i].SetActive(true);
        }
    }
    
    private void SetSlotInvisibility(int start, int end)
    {
        for (int i = start; i <= end; i++)
        {
            CustomLogger.Log(slots[i].name);
            slots[i].SetActive(false);
        }
    }

    private void reset()
    {
        unitGameManager.ResetList();
    }
    
}