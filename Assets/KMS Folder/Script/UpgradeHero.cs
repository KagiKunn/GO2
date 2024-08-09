using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UpgradeHero : MonoBehaviour
{
    public TMP_Text heroName;
    public TMP_Text heroHP;
    public TMP_Text heroAttack;
    public TMP_Text heroAttackSpeed;
    public Image heroCharacterImage;
    public Button closeBtn;
    
    private HeroData upgradeHero;
    
    void OnEnable()
    {
        HeroGameManager heroGameManager = HeroGameManager.Instance;
        if (heroGameManager != null)
        {
            upgradeHero = heroGameManager.GetUpgradeHero();
            CustomLogger.Log("Upgrade Hero Loaded" + (upgradeHero != null ? upgradeHero.Name : "Null"));
        }
        
        if (upgradeHero != null)
        {
            DisplayHeroInfo();
        }
        else
        {
            CustomLogger.Log("No hero selected for upgrade.");
        }
        closeBtn.onClick.AddListener(OnCloseBtn);
    }

    private void DisplayHeroInfo()
    {
        heroName.text = upgradeHero.Name;
        heroHP.text = $"HP: {upgradeHero.HP}";
        heroAttack.text = $"Attack: {upgradeHero.Attack}";
        heroAttackSpeed.text = $"Attack Speed: {upgradeHero.AttackSpeed}";
        heroCharacterImage.sprite = upgradeHero.CharacterImg;
    }
    
    private void OnCloseBtn()
    {
        // HeroGameManager의 upgradeHero를 초기화
        HeroGameManager heroGameManager = FindFirstObjectByType<HeroGameManager>();
        if (heroGameManager != null)
        {
            heroGameManager.ClearUpgradeHero();
        }

        // 이전 씬으로 돌아가기
        SceneManager.LoadScene("HeroManagement");
    }
}
