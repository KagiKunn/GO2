using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CastleWall : MonoBehaviour
{
    [SerializeField] public float maxHealth = 3000; // 성벽의 최대 체력
    [SerializeField] public float health; // 성벽의 현재 체력
    [SerializeField] public float maxShield = 1000; // 실드의 최대 값
    [SerializeField] public float shield; // 현재 실드 값

    [SerializeField] private Canvas gameOverCanvas; // 겜오버 캔버스 참조변수
    [SerializeField] private Image gameOverImage; // 겜오버 이미지 참조변수
    [SerializeField] private Button gameOverButton; // 겜오버 버튼 참조변수
    [SerializeField] private Slider healthSlider; // 체력 슬라이더 참조변수
    [SerializeField] private Slider shieldSlider; // 실드 슬라이더 참조변수

    public bool hasShield; // hasShield를 public으로 유지

    [SerializeField] private bool activateShield;
    [SerializeField] private float activateShieldValue = 80f; // ActivateShield 메서드에서 설정할 실드 값

    private Coroutine earnShieldCoroutine;

    private void Awake()
    {
        // 슬라이더 초기화
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }

        if (shieldSlider != null)
        {
            shieldSlider.maxValue = maxShield; // 실드 슬라이더의 최대값을 설정
            shieldSlider.value = shield;
        }
        else
        {
            Debug.LogError("Shield Slider is not assigned in the Inspector!");
        }
    }

    private void Start()
    {
        health = maxHealth;
        shield = 0; // 초기에는 실드가 없는 상태
        hasShield = false;
        activateShield = false;

        if (gameOverCanvas != null)
        {
            gameOverCanvas.enabled = false;
        }

        if (gameOverImage != null)
        {
            gameOverImage.enabled = false;
        }

        if (gameOverButton != null)
        {
            gameOverButton.enabled = false;
            gameOverButton.onClick.AddListener(OnGameOverButtonClick);
        }

        // 슬라이더 초기화 상태 디버깅
        Debug.Log("Start: hasShield set to false, shieldSlider value: " + shieldSlider.value);
    }

    private void Update()
    {
        // 매 프레임마다 hasShield의 상태를 확인하고 슬라이더 값을 업데이트
        if (activateShield)
        {
            ActivateShield();
            activateShield = false; // 한번 실행 후 비활성화
        }

        if (hasShield)
        {
            UpdateShieldSlider();
        }
        else
        {
            if (shield != 0)
            {
                shield = 0;
                UpdateShieldSlider();
            }
        }

        UpdateHealthSlider();
    }

    public void ActivateShield()
    {
        hasShield = true;
        shield = activateShieldValue;
        UpdateShieldSlider();
        Debug.Log("Shield activated with value: " + shield);
    }

    // 성벽이 공격당했을 때 체력을 감소시키는 함수
    public void TakeDamage(int damage)
    {
        if (hasShield)
        {
            shield -= damage;
            if (shield <= 0)
            {
                shield = 0; // 실드가 음수가 되지 않도록 설정
                hasShield = false;
                CustomLogger.Log("보호막이 파괴되었습니다!");
            }
        }
        else
        {
            health -= damage;
            if (health <= 0)
            {
                health = 0; // 체력이 음수가 되지 않도록 설정
                CustomLogger.Log("성벽이 파괴되었습니다!");
                Time.timeScale = 0f; // 게임 일시 정지
                Debug.Log("게임 일시 정지");
                ShowGameOverUI();
                Destroy(gameObject);
            }
        }
    }

    private void UpdateHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }
    }

    private void UpdateShieldSlider()
    {
        if (shieldSlider != null)
        {
            shieldSlider.value = shield;
        }
        else
        {
            Debug.LogWarning("Shield Slider is not assigned in the Inspector when trying to update it!");
        }
    }

    private void ShowGameOverUI()
    {
        Debug.Log("게임오버 UI 출현");
        gameOverCanvas.enabled = true;
        gameOverImage.enabled = true;
        gameOverButton.enabled = true;
    }

    public void OnGameOverButtonClick()
    {
        Debug.Log("버튼 클릭됨");
        SceneManager.LoadScene("Title");
    }

    public void EarnShield(float duration, float shieldAmount)
    {
        Debug.Log("EarnShield called with duration: " + duration + " and shieldAmount: " + shieldAmount);

        // 기존의 코루틴이 실행 중이면 중지합니다.
        if (earnShieldCoroutine != null)
        {
            StopCoroutine(earnShieldCoroutine);
        }

        hasShield = true;
        shield = Mathf.Min(maxShield, shield + shieldAmount); // 실드가 maxShield를 초과하지 않도록 설정
        earnShieldCoroutine = StartCoroutine(ResetEarnShieldAfterDelay(duration));
    }

    private IEnumerator ResetEarnShieldAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hasShield = false;
        shield = 0;
        UpdateShieldSlider();
        Debug.Log("Shield reset to original values for: " + gameObject.name);
    }
}
