using UnityEngine;

public class StageEndUIManager : MonoBehaviour
{
    [SerializeField] private GameObject stageEndUI;

    private void OnEnable()
    {
        DarkElfBossController.OnHorseRootDisabledEvent += ShowStageEndUI;
    }

    private void OnDisable()
    {
        DarkElfBossController.OnHorseRootDisabledEvent -= ShowStageEndUI;
    }

    private void ShowStageEndUI()
    {
        if (stageEndUI != null)
        {
            stageEndUI.SetActive(true);
        }
    }
}