using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillIcon : MonoBehaviour
{
    [SerializeField]
    private UIDocument uiDocument;

    private Button[] skillButtons;

    private void Awake()
    {
        skillButtons = uiDocument.rootVisualElement.Query<Button>("skill-icn").ToList().ToArray();
        
        for (int i = 0; i < skillButtons.Length; i++)
        {
            int index = i; // 로컬 변수로 인덱스를 저장하여 클로저 문제 해결
            skillButtons[i].clicked += () => OnSkillButtonClicked(index);
        }
        
    }
    private void OnSkillButtonClicked(int index)
    {
        Debug.Log("Button " + index + " clicked.");
    }
}
