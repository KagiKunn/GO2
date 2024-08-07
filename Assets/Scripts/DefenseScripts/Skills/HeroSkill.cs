using System.Collections;
using UnityEngine;

public abstract class HeroSkill : ScriptableObject
{
    public Sprite skillIcon;
    public GameObject skillImagePrefab; // 이미지 프리팹 추가

    public bool isActive = true;
    public float cooldown = 10f;
 
    public GameObject effect;
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
            RectTransform skillCanvasRect = skillCanvasObject.GetComponent<RectTransform>();
        
            if (skillCanvas != null)
            {
                // 이미지 오브젝트 생성
                GameObject skillImage = Instantiate(skillImagePrefab, skillCanvas.transform);

                RectTransform rectTransform = skillImage.GetComponent<RectTransform>();

                // 캔버스의 실제 크기를 구합니다.
                Vector2 canvasSize = skillCanvasRect.rect.size; // Canvas의 실제 크기 가져오기
    
                // 오브젝트의 크기를 캔버스 크기에 비례하여 조정합니다.
                float scaleFactor = canvasSize.y * 10f / rectTransform.rect.height; // 오브젝트 높이의 80%로 설정
                rectTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1);

                // 앵커 포인트를 중앙으로 설정
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);

                // 원하는 위치에 배치합니다.
                float outsideRightPositionX = canvasSize.x / 2 + rectTransform.rect.width * scaleFactor / 2;
                rectTransform.anchoredPosition = new Vector2(outsideRightPositionX, 0);

                // 왼쪽으로 이동 시작
                CoroutineRunner.Instance.StartCoroutine(MoveAndPauseImage(skillImage));
            }
        }
    }



    private IEnumerator MoveAndPauseImage(GameObject skillImage)
    {
        RectTransform rectTransform = skillImage.GetComponent<RectTransform>();
        Vector2 targetPosition = new Vector2(rectTransform.anchoredPosition.x - 300, rectTransform.anchoredPosition.y);
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
