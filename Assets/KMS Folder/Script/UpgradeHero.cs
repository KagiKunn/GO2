using System;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
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
    public SimplePopup popup;
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
        LocalizedString localizedString = new LocalizedString
            { TableReference = "UI", TableEntryReference = "HeroHP" };

        localizedString.StringChanged += (localizedText) => {
            heroHP.text = $"{localizedText}: {upgradeHero.OffenceHP}";
        };
        localizedString = new LocalizedString
            { TableReference = "UI", TableEntryReference = "HeroAtk" };

        localizedString.StringChanged += (localizedText) => {
            heroAttack.text = $"{localizedText}: {upgradeHero.OffenceAttack}";
        };
        localizedString = new LocalizedString
            { TableReference = "UI", TableEntryReference = "HeroAtkSp" };

        localizedString.StringChanged += (localizedText) => {
            heroAttackSpeed.text = $"{localizedText}: {upgradeHero.OffenceAttackSpeed}";
        };
    }

    private void DisplayHeroInfo()
    {
        heroName.text = upgradeHero.Name;
        LocalizedString localizedString = new LocalizedString
            { TableReference = "UI", TableEntryReference = "HeroHP" };

        localizedString.StringChanged += (localizedText) => {
            heroHP.text = $"{localizedText}: {upgradeHero.OffenceHP}";
        };
        localizedString = new LocalizedString
            { TableReference = "UI", TableEntryReference = "HeroAtk" };

        localizedString.StringChanged += (localizedText) => {
            heroAttack.text = $"{localizedText}: {upgradeHero.OffenceAttack}";
        };
        localizedString = new LocalizedString
            { TableReference = "UI", TableEntryReference = "HeroAtkSp" };

        localizedString.StringChanged += (localizedText) => {
            heroAttackSpeed.text = $"{localizedText}: {upgradeHero.OffenceAttackSpeed}";
        };
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
        if (PlayerLocalManager.Instance.lMoney < 100)
        {
            popup.ShowPopup("Need More Money!");
            CustomLogger.Log("Need More Money!", "red");
            return;
        }
        PlayerLocalManager.Instance.lMoney -= 100;
        HeroManager.Instance.upgradeHero.OffenceHP += 20;
        HeroManager.Instance.upgradeHero.OffenceAttack += 10;
        HeroManager.Instance.upgradeHero.OffenceAttackSpeed += 5;
        HeroManager.Instance.SaveHeroFormation();
        PlayerLocalManager.Instance.Save();
        popup.ShowPopup("Upgrade Complete!");
    }
}
