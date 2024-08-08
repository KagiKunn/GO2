using System;
using UnityEngine;

public class BossJudge : MonoBehaviour
{
    public GameObject root;

    private void Awake()
    {

    }

    void Update()
    {
        root.SetActive(false);
        if (!root.activeSelf)
        {
            CustomLogger.Log("보스 사망..............","red");
        }
    }
}
