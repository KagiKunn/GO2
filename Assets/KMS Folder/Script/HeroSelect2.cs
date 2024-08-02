using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelect2 : MonoBehaviour
{
    public HeroGameManager heroGameManager;

    public Image CharacterImage;
    public Button[] heroButtons;
    public Button[] selectedHeroSlots;
    public Button resetBtn, saveBtn;
    public GameObject MainHero;

    private List<HeroData> heroes;

    private void Start()
    {
        heroes = heroGameManager.GetHeroes();

        InitializeHeroButtons();
        InitializeSelectedHeroSlots();

        resetBtn.onClick.AddListener(ResetHeroSelection);
        saveBtn.onClick.AddListener(SaveHeroSelection);

        LoadHeroFormation();
    }

    private void InitializeHeroButtons()
    {
        for (int i = 0; i < heroButtons.Length; i++)
        {
            int index = i;
            heroButtons[i].GetComponent<Image>().sprite = heroes[index].ProfileImg;
            heroButtons[i].onClick.AddListener(() => OnHeroButtonClicked(index));
        }
    }

    private void InitializeSelectedHeroSlots()
    {
        for (int i = 0; i < selectedHeroSlots.Length; i++)
        {
            int index = i;
            selectedHeroSlots[i].onClick.AddListener(() => OnSelectedHeroSlotClicked(index));
        }
    }

    private void OnHeroButtonClicked(int index)
    {
        if (GetSelectedHeroes().Count >= 3)
        {
            CustomLogger.Log("Cannot select more than 3 heroes.");
            return;
        }

        HeroData selectedHeroData = heroes[index];

        if (GetSelectedHeroes().Exists(h => h.Name == selectedHeroData.Name))
        {
            CustomLogger.Log("Cannot select the same hero twice.");
            return;
        }

        AddHeroToSlot(selectedHeroData);
    }

    private void AddHeroToSlot(HeroData heroData)
    {
        for (int i = 0; i < selectedHeroSlots.Length; i++)
        {
            Image slotImage = selectedHeroSlots[i].GetComponent<Image>();
            if (slotImage.sprite == null)
            {
                slotImage.sprite = heroData.ProfileImg;
                slotImage.enabled = true;
                SetMainCharacterImage(heroData.CharacterImg);
                AddSelectedHero(heroData);
                break;
            }
        }
    }

    private void OnSelectedHeroSlotClicked(int index)
    {
        Image slotImage = selectedHeroSlots[index].GetComponent<Image>();
        HeroData heroToRemove = GetSelectedHeroes().Find(h => h.ProfileImg == slotImage.sprite);
        if (heroToRemove != null)
        {
            RemoveSelectedHero(heroToRemove);
        }
        slotImage.sprite = null;
        slotImage.enabled = false;
    }

    private void SetMainCharacterImage(Sprite mainSprite)
    {
        if (mainSprite != null)
        {
            CharacterImage.sprite = mainSprite;
            MainHero.SetActive(true);
        }
        else
        {
            CharacterImage.sprite = null;
            MainHero.SetActive(false);
        }
    }

    private void ResetHeroSelection()
    {
        foreach (var slot in selectedHeroSlots)
        {
            Image slotImage = slot.GetComponent<Image>();
            slotImage.sprite = null;
            slotImage.enabled = false;
        }
        GetSelectedHeroes().Clear();
        SetMainCharacterImage(null);
    }

    private void SaveHeroSelection()
    {
        heroGameManager.SaveHeroFormation();
    }

    private void LoadHeroFormation()
    {
        heroGameManager.LoadHeroFormation();
        List<HeroData> selectedHeroes = GetSelectedHeroes();
        for (int i = 0; i < selectedHeroes.Count; i++)
        {
            if (i < selectedHeroSlots.Length)
            {
                Image slotImage = selectedHeroSlots[i].GetComponent<Image>();
                slotImage.sprite = selectedHeroes[i].ProfileImg;
                slotImage.enabled = true;
            }
        }
        if (selectedHeroes.Count > 0)
        {
            SetMainCharacterImage(selectedHeroes[0].CharacterImg);
        }
        else
        {
            SetMainCharacterImage(null);
        }
    }

    // 영웅을 선택된 영웅 목록에 추가하는 메서드
    private void AddSelectedHero(HeroData hero)
    {
        if (GetSelectedHeroes().Count < 3 && !GetSelectedHeroes().Exists(h => h.Name == hero.Name))
        {
            GetSelectedHeroes().Add(hero);
        }
    }

    // 영웅을 선택된 영웅 목록에서 제거하는 메서드
    private void RemoveSelectedHero(HeroData hero)
    {
        GetSelectedHeroes().Remove(hero);
    }

    // 선택된 영웅 목록을 반환하는 메서드
    private List<HeroData> GetSelectedHeroes()
    {
        return heroGameManager.GetSelectedHeroes();
    }
}
