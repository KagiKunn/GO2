using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CastleWall : MonoBehaviour
{
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private Image gameOverImage;
    [SerializeField] private Button gameOverButton;

    private Tilemap castleTile;
    private Color originColor;

    private void Awake()
    {
        castleTile = GetComponent<Tilemap>();
        originColor = castleTile.color;

        if (gameOverImage != null)
        {
            gameOverImage.gameObject.SetActive(false);
        }
    }

    private void Start()
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

    public void TakeDamage(float damage)
    {
        CastleWallManager.Instance.ApplyDamage(damage);

        if (CastleWallManager.Instance.GetHealth() <= 0)
        {
            ShowGameOverUI();
            Destroy(gameObject);
        }
    }

    public void EarnShield(float duration, float shieldAmount)
    {
        Debug.Log("EarnShield called with duration: " + duration + " and shieldAmount: " + shieldAmount);
        ChangeWallColor(true);  // 실드 활성화 시 색상 변경
        CastleWallManager.Instance.EarnShield(duration, shieldAmount);
    }

    private void ChangeWallColor(bool activateShield)
    {
        if (activateShield)
        {
            castleTile.color = Color.cyan;
        }
        else
        {
            castleTile.color = originColor;
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
}
