using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Audio;

#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.

public class CastleWallManager : MonoBehaviour {
    public static CastleWallManager Instance;

    [SerializeField] private float activateShieldValue = 80f; // 기본 실드 최대값

    private HeroManager heroManager;
    private Coroutine resetShieldCoroutine;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage; // 슬라이더의 Fill Image를 참조
    [SerializeField] private Image handleImage; // 슬라이더의 Handle Image를 참조
    [SerializeField] private Sprite shieldIcon; // 실드 활성화 시 사용할 아이콘
    [SerializeField] private Sprite healthIcon; // 기본 체력 아이콘

    [SerializeField] private bool hasShield;
    private float shieldAmount;

    private StageC stageC;
    private List<GameObject> wallObjects;

    public float maxHealth;
    public float health;
    public float shield;
    public AudioClip soundClip;
    private AudioSource audioSource;
    public AudioMixerGroup sfxMixerGroup;
    private void Awake() {
        Debug.Log("캐슬월에서 가져온 스테이지카운트 : " + PlayerLocalManager.Instance.lStage);
        if (PlayerLocalManager.Instance.lStage == 1) {
            
            health = maxHealth;
        }
        // Load game data
        if (PlayerLocalManager.Instance != null) {
            maxHealth = PlayerLocalManager.Instance.lCastleMaxHp;
            health = PlayerLocalManager.Instance.lCastleHp;
        }

        wallObjects = new List<GameObject>();
        wallObjects.Add(GameObject.FindGameObjectsWithTag("RightWall")[0]);
        wallObjects.Add(GameObject.FindGameObjectsWithTag("LeftWall")[0]);

        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        shield = 0f;
        hasShield = false;

        InitializeSlider();

        stageC = FindObjectOfType<StageC>();
    }

    private void InitializeSlider() {
        if (healthSlider != null) {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
            fillImage.color = Color.green; // 기본 색상
            handleImage.sprite = healthIcon; // 기본 체력 아이콘 설정
        }
    }
    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = soundClip;
        audioSource.outputAudioMixerGroup = sfxMixerGroup;
    }
    private void Update() {
        if (!hasShield) {
            foreach (var wall in wallObjects) {
                if (wall != null) {
                    CastleWall cwall = wall.GetComponent<CastleWall>();
                    cwall.ChangeWallColor(false); // 실드 비활성화 시 색상 변경
                }
            }
        }

        // hasShield가 false가 되었을 때 실드 비활성화
        if (!hasShield && shield > 0) {
            SetShield(0); // 실드를 비활성화하고 초기화
            Debug.Log("Shield deactivated.");
        }
    }
    public void PlaySound()
    {
        if (audioSource != null && soundClip != null)
        {
            audioSource.Play();
        }
    }
    public void ApplyDamage(float damage) {
        if (hasShield) {
            shield -= damage;

            if (shield <= 0) {
                damage = -shield; // 남은 데미지를 체력에 적용
                shield = 0;
                hasShield = false;

                Debug.Log("Shield destroyed!");
            } else {
                damage = 0;
            }
        }

        health -= damage;

        if (health <= 0) {
            health = 0;
            HandleGameOver();
        }

        UpdateSliders();
    }

    public void EarnShield(float duration, float shieldAmount) {
        Debug.Log("EarnShield called with duration: " + duration + " and shieldAmount: " + shieldAmount);

        // 슬라이더의 최대값을 전달받은 shieldAmount로 설정
        activateShieldValue = shieldAmount;

        hasShield = true;

        if (resetShieldCoroutine != null) {
            StopCoroutine(resetShieldCoroutine);
        }

        foreach (var wall in wallObjects) {
            CastleWall cwall = wall.GetComponent<CastleWall>();
            cwall.ChangeWallColor(true);
        }

        AddShield(shieldAmount);
        resetShieldCoroutine = StartCoroutine(ResetEarnShieldAfterDelay(duration));

        // 실드가 활성화될 때 실드 양을 로그로 출력
        Debug.Log("Shield activated with shield amount: " + shield);
    }

    private IEnumerator ResetEarnShieldAfterDelay(float delay) {
        if (!hasShield) {
            yield break;
        }

        yield return new WaitForSeconds(delay);

        foreach (var wall in wallObjects) {
            CastleWall cwall = wall.GetComponent<CastleWall>();
            cwall.ChangeWallColor(false);
        }

        SetShield(0);
        hasShield = false;
        Debug.Log("Shield reset to original values.");
    }

    public void AddShield(float shieldAmount) {
        shield = Mathf.Min(activateShieldValue, shield + shieldAmount);
        hasShield = shield > 0;
        UpdateSliders();
        Debug.Log("Shield added. Current shield: " + shield);
    }

    public void SetShield(float shieldValue) {
        shield = Mathf.Clamp(shieldValue, 0, activateShieldValue);
        hasShield = shield > 0;
        UpdateSliders();
        Debug.Log("Shield set to value: " + shield);
    }

    private void UpdateSliders() {
        if (healthSlider != null) {
            if (hasShield) {
                // 실드가 활성화된 경우: 슬라이더를 실드 값으로 설정
                healthSlider.maxValue = activateShieldValue;
                healthSlider.value = shield;
                fillImage.color = Color.blue; // 실드 색상
                handleImage.sprite = shieldIcon; // 실드 아이콘
            } else {
                // 실드가 비활성화된 경우: 슬라이더를 체력 값으로 설정
                healthSlider.maxValue = maxHealth;
                healthSlider.value = health;
                fillImage.color = Color.red; // 체력 색상
                handleImage.sprite = healthIcon; // 체력 아이콘
            }
        }
    }

    private void HandleGameOver()
    {
        PlaySound();
        DestroyWallsWithTag("TagForDestroyWalls");

        Debug.Log("성벽이 파괴되었습니다! 게임 오버!");

        if (stageC != null) {
            stageC.ShowGameOverUI();
        } else {
            Debug.LogWarning("StageC instance not found.");
        }

        HeroManager.Instance.ClearHeroFormation();
    }

    private void DestroyWallsWithTag(string tag) {
        GameObject[] walls = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject wall in walls) {
            Destroy(wall);
        }
    }

    public void SaveWallHP() {
        CustomLogger.Log("성벽 HP 정보 저장됨 health, mH, eH" + health + "," + maxHealth);
        PlayerLocalManager.Instance.lCastleHp = health;
        PlayerLocalManager.Instance.lCastleMaxHp = maxHealth;
        PlayerLocalManager.Instance.Save();
    }

    public float GetHealth() => health;
    public float GetShield() => shield;
}
