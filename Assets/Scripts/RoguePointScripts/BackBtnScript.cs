using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackBtnScript : MonoBehaviour
{
    private string filePath;

    public Button backButton;  // 저장 버튼
    void Awake()
    {
        
        // 버튼에 클릭 이벤트 리스너 추가
        backButton.onClick.AddListener(() => GoToMain());
    }

    private void GoToMain()
    {
        SceneManager.LoadScene("Title");
    }
}