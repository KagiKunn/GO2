using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelect2 : MonoBehaviour
{
    public NoticeUI _notice;
    public HeroGameManager heroGameManager;
    public Image CharacterImage;
    public Button[] heroButtons;
    public Button[] selectedHeroSlots;
    public Button resetBtn, saveBtn;

    private List<HeroData> heroes;

    void Awake()
    {
        _notice = FindFirstObjectByType<NoticeUI>();
    }

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
        // Ensure we do not exceed the bounds of the heroButtons array or heroes list
        int buttonCount = Mathf.Min(heroButtons.Length, heroes.Count);
        for (int i = 0; i < buttonCount; i++)
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
        if (heroGameManager.GetSelectedHeroes().Count >= 3) return;

        HeroData selectedHeroData = heroes[index];

        if (heroGameManager.GetSelectedHeroes().Exists(h => h.Name == selectedHeroData.Name)) return;

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
                heroGameManager.AddSelectedHero(heroData);
                break;
            }
        }
    }

    private void OnSelectedHeroSlotClicked(int index)
    {
        Image slotImage = selectedHeroSlots[index].GetComponent<Image>();
        if (slotImage.sprite != null)
        {
            heroGameManager.GetSelectedHeroes().RemoveAll(h => h.ProfileImg == slotImage.sprite);
            slotImage.sprite = null;
            slotImage.enabled = false;
        }
    }

    private void SetMainCharacterImage(Sprite mainSprite)
    {
        CharacterImage.sprite = mainSprite;
        CharacterImage.gameObject.SetActive(mainSprite != null);
    }

    private void ResetHeroSelection()
    {
        foreach (var slot in selectedHeroSlots)
        {
            Image slotImage = slot.GetComponent<Image>();
            slotImage.sprite = null;
            slotImage.enabled = false;
        }
        heroGameManager.ClearHeroFormation();
        SetMainCharacterImage(null);
    }

    private void SaveHeroSelection()
    {
        heroGameManager.SaveHeroFormation();
        _notice.SUB("Save Successefully!");
    }

    private void LoadHeroFormation()
    {
        heroGameManager.LoadHeroFormation();
        List<HeroData> selectedHeroes = heroGameManager.GetSelectedHeroes();
        for (int i = 0; i < selectedHeroes.Count; i++)
        {
            if (i < selectedHeroSlots.Length)
            {
                Image slotImage = selectedHeroSlots[i].GetComponent<Image>();
                slotImage.sprite = selectedHeroes[i].ProfileImg;
                slotImage.enabled = true;
            }
        }
        SetMainCharacterImage(selectedHeroes.Count > 0 ? selectedHeroes[0].CharacterImg : null);
    }
}
