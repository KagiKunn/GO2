using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public abstract class HeroSkill : MonoBehaviour
{
    public Sprite skillIcon;
    public GameObject skillImagePrefab; // 이미지 프리팹 추가
    public Button skillButton; // 버튼 참조 추가
    public SkillPanelManager skillPanelManager; // SkillPanelManager 참조 추가

    public AudioClip soundClip;
    private AudioSource skillAudioSource;

    public AudioMixerGroup sfxMixerGroup;
    
    public bool isActive = true;
    public float cooldown = 10f;
    public float extraCool = 0;

    private float originTime;
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
        originTime = Time.timeScale;
        StartCoroutine(CreateSkillImage());
    }

    private void Awake()
    {
        extraCool = GameObject.Find("InitSetting").GetComponent<DefenseInit>().extraCool1;
        cooldown = cooldown - (cooldown * (extraCool/10));
    }

    private void Start()
    {
        skillAudioSource = gameObject.AddComponent<AudioSource>();
        skillAudioSource.clip = soundClip;
        skillAudioSource.outputAudioMixerGroup = sfxMixerGroup;
    }
    
    public void PlaySound()
    {
        if (skillAudioSource != null && soundClip != null)
        {
            skillAudioSource.Play();
        }
    }

    private IEnumerator CreateSkillImage()
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
                float scaleFactor = canvasSize.y * 15f / rectTransform.rect.height;
                rectTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1);

                // 앵커 포인트를 중앙으로 설정
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);

                // 원하는 위치에 배치합니다.
                float outsideRightPositionX = canvasSize.x / 2 + rectTransform.rect.width * scaleFactor / 2;
                rectTransform.anchoredPosition = new Vector2(outsideRightPositionX, 0);

                // 시간을 멈추기
                Time.timeScale = 0.5f;
                PlaySound();
                // 왼쪽으로 이동 시작
                yield return StartCoroutine(MoveAndPauseImage(skillImage));
            }
        }
        OnSkillImageComplete();
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
            elapsedTime += Time.unscaledDeltaTime; // 시간 정지 시에도 애니메이션이 진행되도록 수정
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;

        // 1초간 멈추기 (시간 정지)
        yield return new WaitForSecondsRealtime(1.0f);

        // 오브젝트 삭제
        Destroy(skillImage);

        // 시간을 원래대로 돌리기
        Time.timeScale = originTime;
    }

    protected virtual void OnSkillImageComplete()
    {
        // 자식 클래스에서 오버라이드 가능
    }
    public void ResetCooldownText()
    {
        if (skillButton != null)
        {
            skillButton.text = ""; // 텍스트 초기화
        }
    }
}
