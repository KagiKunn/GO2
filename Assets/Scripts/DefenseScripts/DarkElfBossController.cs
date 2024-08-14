using System;
using UnityEngine;

public class DarkElfBossController : MonoBehaviour
{
    public static event Action OnHorseRootDisabledEvent;

    private GameObject HorseRoot;

    private void Awake()
    {
        // HorseRoot 오브젝트 찾기
        Transform horseRootTransform = transform.Find("HorseRoot");
        if (horseRootTransform != null)
        {
            HorseRoot = horseRootTransform.gameObject;
        }
        else
        {
            Debug.LogError("HorseRoot not found in DarkElfBoss");
        }
    }

    private void Update()
    {
        // HorseRoot가 비활성화되었는지 확인
        if (HorseRoot != null && !HorseRoot.activeInHierarchy)
        {
            OnHorseRootDisabledEvent?.Invoke();
            // 이벤트 발생 후 더 이상 중복 발생하지 않도록 스크립트 비활성화
            enabled = false;
        }
    }
}