using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    private bool isClick;
    private int count;

    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void Init()
    {
        this.isClick = false;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name.Equals("Title"))
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !isClick)
            {
                StartCoroutine(CrQuitTimer());
                isClick = true;

                count++;

                if (count >= 2)
                {
                    count = 0;
                    isClick = false;
                    CustomLogger.LogWarning("Exit");
                    Application.Quit();
                }
            }
        }
    }

    IEnumerator CrQuitTimer()
    {
        yield return new WaitForSeconds(0.1f);
        this.isClick = false;

        yield return new WaitForSeconds(0.3f);
        this.isClick = false;
        this.count = 0;
    }
}