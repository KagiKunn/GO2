using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class CastleWallManager : MonoBehaviour
{
    public static CastleWallManager Instance;

    [SerializeField] private float maxHealth = 3000f;
    [SerializeField] private float activateShieldValue = 80f; // 실드 활성화 시 설정할 값

    public float health;
    public float shield;
    public bool activateShield; //activateShield가 true이면 실드 적용 + hasShield를 true로 변경
    private Coroutine resetShieldCoroutine;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider shieldSlider;

    [SerializeField] private bool hasShield; // hasShield가 false가 되면 즉시 실드 무효화

    [SerializeField] private Canvas gameOverCanvas; // 게임오버 UI 관련 참조
    [SerializeField] private Image gameOverImage;
    [SerializeField] private Button gameOverButton;

    private void Awake()
    {
        // Singleton 패턴을 사용하여 유일한 인스턴스 보장
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        health = maxHealth;
        shield = 0f;
        hasShield = false;
        activateShield = false;

        InitializeSliders();
        InitializeGameOverUI();
    }

    private void InitializeSliders()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }

        if (shieldSlider != null)
        {
            shieldSlider.maxValue = activateShieldValue; // 실드 슬라이더의 최대값을 activateShieldValue로 설정
            shieldSlider.value = shield;
        }
    }

    private void InitializeGameOverUI()
    {
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
    }

    private void Update()
    {
        // activateShield가 true로 설정되었을 때 실드 활성화
        if (activateShield)
        {
            ActivateShield();
            activateShield = false; // 실행 후 비활성화
        }

        // hasShield가 false가 되었을 때 실드 비활성화
        if (!hasShield && shield > 0)
        {
            SetShield(0); // 실드를 비활성화하고 초기화
            Debug.Log("Shield deactivated.");
        }
    }

    public void ApplyDamage(float damage)
    {
        if (hasShield)
        {
            shield -= damage;
            if (shield <= 0)
            {
                damage = -shield; // 남은 데미지를 체력에 적용
                shield = 0;
                hasShield = false;
                Debug.Log("Shield destroyed!");
            }
            else
            {
                damage = 0;
            }
        }

        health -= damage;
        if (health <= 0)
        {
            health = 0;
            HandleGameOver();
        }

        UpdateSliders();
    }

    public void EarnShield(float duration, float shieldAmount)
    {
        Debug.Log("EarnShield called with duration: " + duration + " and shieldAmount: " + shieldAmount);

        // 기존의 실드 초기화 코루틴이 실행 중이면 중지
        if (resetShieldCoroutine != null)
        {
            StopCoroutine(resetShieldCoroutine);
        }

        AddShield(shieldAmount); // 실드 추가
        resetShieldCoroutine = StartCoroutine(ResetEarnShieldAfterDelay(duration)); // 일정 시간 후 실드 초기화
    }

    private IEnumerator ResetEarnShieldAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetShield(0); // 실드 초기화
        hasShield = false; // 실드 비활성화
        Debug.Log("Shield reset to original values.");
    }

    public void ActivateShield()
    {
        SetShield(activateShieldValue);
        hasShield = true; // 실드 활성화 시 hasShield를 true로 설정
        Debug.Log("Shield activated with value: " + activateShieldValue);
    }

    public void SetActivateShield(bool value)
    {
        activateShield = value;
    }

    public void AddShield(float shieldAmount)
    {
        shield = Mathf.Min(activateShieldValue, shield + shieldAmount);
        hasShield = shield > 0;
        UpdateSliders();
        Debug.Log("Shield added. Current shield: " + shield);
    }

    public void SetShield(float shieldValue)
    {
        shield = Mathf.Clamp(shieldValue, 0, activateShieldValue);
        hasShield = shield > 0;
        UpdateSliders();
        Debug.Log("Shield set to value: " + shield);
    }

    private void UpdateSliders()
    {
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }

        if (shieldSlider != null)
        {
            shieldSlider.maxValue = activateShieldValue; // 실드 슬라이더의 최대값을 최신 activateShieldValue로 설정
            shieldSlider.value = shield;
        }
    }

    private void HandleGameOver()
    {
        Debug.Log("성벽이 파괴되었습니다! 게임 오버!");

        if (gameOverCanvas != null)
        {
            gameOverCanvas.enabled = true;
        }

        if (gameOverImage != null)
        {
            gameOverImage.enabled = true;
        }

        if (gameOverButton != null)
        {
            gameOverButton.enabled = true;
        }

        Time.timeScale = 0f; // 게임 일시 정지
    }

    public void OnGameOverButtonClick()
    {
        Debug.Log("버튼 클릭됨");
        SceneManager.LoadScene("Title");
    }

    public float GetHealth() => health;
    public float GetShield() => shield;
}