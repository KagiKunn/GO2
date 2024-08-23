using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillPanelManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    public GameObject[] heroSkillArray;

    private VisualElement bottomPanel;
    
    private bool isPanelVisible = true;
    private float hiddenPosition = -120f; // 패널 숨김 위치 (패널 높이 만큼)
    private float visiblePosition = 0f;   // 패널 보임 위치 (화면 하단)
    
    private string filePath;
    private string jsonData;

    private List<string> selectedHeroes = new List<string>();

    private void Awake()
    {
        List<HeroData> heroList = HeroManager.Instance.selectedHeroes;

        for (int i = 0; i < heroList.Count; i++)
        {
            HeroData hero = heroList[i];
            CustomLogger.Log(hero.Name, "red");
            heroSkillArray[i] = Resources.Load<GameObject>("Defense/Hero/" + hero.Name);

            if (hero != null)
            {
                selectedHeroes.Add(hero.Name);
                CustomLogger.Log(hero.Name);
            }
        }

        // 나머지 초기화 코드...
        VisualElement root = uiDocument.rootVisualElement;

        // 스킬 패널 요소 가져오기
        bottomPanel = root.Q<VisualElement>("skill-panel");
        if (bottomPanel == null)
        {
            Debug.LogError("Skill panel not found.");
            return;
        }

        // 패널 초기 위치 설정
        bottomPanel.style.bottom = visiblePosition;

        // 패널 클릭 이벤트 등록
        bottomPanel.RegisterCallback<ClickEvent>(ev => TogglePanel());

        // 패널의 자식 요소들에 대해 이벤트 등록
        RegisterChildClickEvents(bottomPanel);

        // 기존 UI 요소를 설정
        SetupSkillButtons(bottomPanel);
    }

    private void RegisterChildClickEvents(VisualElement parent)
    {
        foreach (var child in parent.Children())
        {
            child.RegisterCallback<ClickEvent>(ev => ev.StopPropagation());

            // 자식 요소가 또 다른 자식 요소를 가질 경우 재귀적으로 처리
            if (child.childCount > 0)
            {
                RegisterChildClickEvents(child);
            }
        }
    }

    private void SetupSkillButtons(VisualElement root)
    {
        // 버튼을 찾습니다.
        var buttons = new Button[3];
        buttons[0] = root.Q<Button>("skill-icn1");
        buttons[1] = root.Q<Button>("skill-icn2");
        buttons[2] = root.Q<Button>("skill-icn3");

        // 각 버튼이 null이 아닌지 확인합니다.
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
            {
                Debug.LogError($"Button {i} is not found.");
                return;
            }
        }

        // 버튼 텍스트를 ScriptableObject 값으로 설정하고 클릭 이벤트를 추가합니다.
        for (int i = 0; i < buttons.Length && i < heroSkillArray.Length; i++)
        {
            int index = i; // 로컬 변수로 인덱스를 캡처합니다.
            GameObject hero = Instantiate(heroSkillArray[index]);
            HeroSkill heroSkill = hero.GetComponent<HeroSkill>();
            buttons[i].clicked += () => OnButtonClicked(heroSkill);

            // 버튼 배경 이미지를 설정합니다.
            HeroSkill skillIcon = heroSkill;
            if (skillIcon != null && skillIcon.skillIcon != null)
            {
                buttons[i].style.backgroundImage = new StyleBackground(skillIcon.skillIcon);
            }

            // 버튼에 HeroSkill의 참조를 설정합니다.
            heroSkill.skillButton = buttons[i];

            // SkillPanelManager 참조 설정
            heroSkill.skillPanelManager = this;

            // 호버 및 클릭 이벤트 추가
            AddHoverAndClickEvents(buttons[i]);
        }
    }

    private void AddHoverAndClickEvents(Button button)
    {
        Color hoverColor = new Color(0.8f, 0.8f, 0.8f); // 회색
        Color clickColor = new Color(0.5f, 0.5f, 0.5f); // 더 진한 회색
        Color originalColor = new Color(1f, 1f, 1f);

        // 호버 이벤트
        button.RegisterCallback<MouseEnterEvent>(ev => { button.style.backgroundColor = hoverColor; });

        // 호버 종료 이벤트
        button.RegisterCallback<MouseLeaveEvent>(ev => { button.style.backgroundColor = originalColor; });

        // 클릭 이벤트
        button.RegisterCallback<ClickEvent>(ev => { button.style.backgroundColor = clickColor; });
    }

    public void UpdateSkillButtonCooldown(Button button, float cooldown)
    {
        button.text = cooldown.ToString("F0");
        if (cooldown < 1)
        {
            button.text = "";
        }
    }

    private void TogglePanel()
    {
        // 패널의 현재 위치에 따라 슬라이드 위치 변경
        if (isPanelVisible)
        {
            StartCoroutine(SlideToPosition(hiddenPosition));
        }
        else
        {
            StartCoroutine(SlideToPosition(visiblePosition));
        }

        isPanelVisible = !isPanelVisible;
    }

    private IEnumerator SlideToPosition(float targetPosition)
    {
        float start = bottomPanel.style.bottom.value.value; // 현재 bottom 값을 직접 가져옴
        float elapsedTime = 0f;
        float duration = 0.25f; // 슬라이드 애니메이션 지속 시간

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newBottom = Mathf.Lerp(start, targetPosition, elapsedTime / duration);
            bottomPanel.style.bottom = newBottom;
            yield return null;
        }

        // 최종 위치를 정확히 설정
        bottomPanel.style.bottom = targetPosition;
    }

    void OnButtonClicked(HeroSkill heroSkill)
    {
        if (StageC.Instance != null && StageC.Instance.isGamePaused) {
            return; // 게임이 일시 정지된 상태에서는 클릭을 무시함
        }
        heroSkill.HeroSkillStart();
    }
}