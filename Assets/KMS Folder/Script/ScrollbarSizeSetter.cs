using System;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarSizeSetter : MonoBehaviour
{
    // HeroManagement Scene에서 스크롤 바 사이즈 조절을 위해 만든 스크립트 입니다.
    public Scrollbar scrollbar;

    void Start()
    {
        InitScrollbar();
    }

    // 초기화 메서드
    public void InitScrollbar()
    {
        scrollbar.size = 0f;
    }
    void Update()
    {
        scrollbar.size = 0f;
    }
    void LateUpdate()
    {
        scrollbar.size = 0f;
    }   
    
    void OnDisable()
    {
        // Scrollbar의 사이즈를 0으로 계속 고정
        scrollbar.size = 0f;
    }
    
}