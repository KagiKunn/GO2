using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class HeroSkill : ScriptableObject
{
    public Sprite skillIcon;
    public GameObject skillImagePrefab; // 이미지 프리팹 추가

    public bool isActive = true;
    public float cooldown = 10f;
    public virtual void HeroSkillAction()
    {
        Time.timeScale /= 2;
        CustomLogger.Log("Action Skill activated");
    }

    public virtual void HeroSkillEffect()
    {
        CustomLogger.Log("Effect Skill activated");
    }

    public virtual void HeroSkillStart()
    {
        CustomLogger.Log("스킬 발동!!!!!!");
        // 이미지 오브젝트 생성 및 위치 설정
        CreateSkillImage();
    }

    private void CreateSkillImage()
    {
        // 특정 태그를 가진 Canvas 찾기
        GameObject skillCanvasObject = GameObject.FindWithTag("SkillCanvas");
        if (skillCanvasObject != null && skillImagePrefab != null)
        {
            Canvas skillCanvas = skillCanvasObject.GetComponent<Canvas>();
            if (skillCanvas != null)
            {
                // 이미지 오브젝트 생성
                GameObject skillImage = Instantiate(skillImagePrefab, skillCanvas.transform);

                RectTransform rectTransform = skillImage.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(1300f, rectTransform.anchoredPosition.y);

                // 왼쪽으로 이동 시작
                CoroutineRunner.Instance.StartCoroutine(MoveAndPauseImage(skillImage));
            }
        }
    }

    private IEnumerator MoveAndPauseImage(GameObject skillImage)
    {
        RectTransform rectTransform = skillImage.GetComponent<RectTransform>();
        Vector2 targetPosition = new Vector2(rectTransform.anchoredPosition.x - 600, rectTransform.anchoredPosition.y);
        float duration = 0.5f;

        Vector2 startPosition = rectTransform.anchoredPosition;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;

        // 1초간 멈추기
        yield return new WaitForSeconds(1.0f);

        // 오브젝트 삭제
        Destroy(skillImage);
    }
}
