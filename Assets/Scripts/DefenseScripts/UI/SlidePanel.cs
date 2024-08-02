using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class SlidePanel : MonoBehaviour
{
    [SerializeField]
    private UIDocument uiDocument;

    private VisualElement bottomPanel;
    private bool isPanelVisible = false;
    private float hiddenPosition = -100f; // 패널 숨김 위치 (패널 높이 만큼)
    private float visiblePosition = 0f;   // 패널 보임 위치 (화면 하단)

    private void Awake()
    {
        var root = uiDocument.rootVisualElement;

        // 패널 요소 가져오기
        bottomPanel = root.Q<VisualElement>("skill-panel");

        // 패널 초기 위치 설정
        bottomPanel.style.bottom = visiblePosition;

        // 패널 클릭 이벤트 등록
        bottomPanel.RegisterCallback<ClickEvent>(ev => TogglePanel());

        // 패널의 자식 요소들에 대해 이벤트 등록
        RegisterChildClickEvents(bottomPanel);
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
}
