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

    [SerializeField] private bool _hasShield;
    
    //hasShield가 true가 되면 max실드 값 대비 설정한 실드 값으로 게이지를 설정
    public bool hasShield
    {
        get { return _hasShield; }
        set
        {
            _hasShield = value;
            if (!_hasShield)
            {
                shield = 0;
                UpdateShieldSlider();
            }
            else
            {
                shieldSlider.maxValue = maxShield;
                shieldSlider.value = shield; // 실드 슬라이더 값 업데이트
            }
            Debug.Log("hasShield set to: " + _hasShield + ", shieldSlider value: " + shieldSlider.value);
        }
    }

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

    // 성벽이 공격당했을 때 체력을 감소시키는 함수
    public void TakeDamage(int damage)
    {
        if (hasShield)
        {
            shield -= damage;
            UpdateShieldSlider();

            if (shield <= 0)
            {
                hasShield = false;
                shield = 0; // 실드가 음수가 되지 않도록 설정
                CustomLogger.Log("보호막이 파괴되었습니다!");
            }
        }
        else
        {
            health -= damage;
            UpdateHealthSlider();

            if (health <= 0)
            {
                health = 0; // 체력이 음수가 되지 않도록 설정
                CustomLogger.Log("성벽이 파괴되었습니다!");
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
        UpdateShieldSlider();
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