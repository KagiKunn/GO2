using System;
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
        HeroManager heroManager = HeroManager.Instance;
        if (heroManager != null)
        {
            upgradeHero = heroManager.GetUpgradeHero();
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

    private void Update()
    {
        heroHP.text = $"HP: {upgradeHero.OffenceHP}";
        heroAttack.text = $"Attack: {upgradeHero.OffenceAttack}";
        heroAttackSpeed.text = $"Attack Speed: {upgradeHero.OffenceAttackSpeed}";
    }

    private void DisplayHeroInfo()
    {
        heroName.text = upgradeHero.Name;
        heroHP.text = $"HP: {upgradeHero.OffenceHP}";
        heroAttack.text = $"Attack: {upgradeHero.OffenceAttack}";
        heroAttackSpeed.text = $"Attack Speed: {upgradeHero.OffenceAttackSpeed}";
        heroCharacterImage.sprite = upgradeHero.CharacterImg;
    }
    
    private void OnCloseBtn()
    {
        // HeroGameManager의 upgradeHero를 초기화
        HeroManager heroManager = FindFirstObjectByType<HeroManager>();
        if (heroManager != null)
        {
            heroManager.ClearUpgradeHero();
        }

        // 이전 씬으로 돌아가기
        SceneManager.LoadScene("HeroManagement");
    }

    public void OnUpgradeBtn()
    {
        HeroManager.Instance.upgradeHero.OffenceHP += 20;
        HeroManager.Instance.upgradeHero.OffenceAttack += 10;
        HeroManager.Instance.upgradeHero.OffenceAttackSpeed += 5;
        HeroManager.Instance.SaveHeroFormation();
    }
}
