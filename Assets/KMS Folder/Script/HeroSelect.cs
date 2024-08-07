using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HeroSelect : MonoBehaviour
{
    public NoticeUI _notice;
    public HeroGameManager heroGameManager;
    public Image CharacterImage;
    public Button[] heroButtons;
    public Button[] selectedHeroSlots;
    public Button resetBtn, saveBtn, reinforceBtn;

    private List<HeroData> heroes;
    
    // 추가된 부분
    private int clickedHeroIndex = -1;
    private Sprite clickedHeroProfileImg;

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
        reinforceBtn.onClick.AddListener(ReinforceBtnClicked);

        LoadHeroFormation();
    }

    private void InitializeHeroButtons()
    {
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
        
        // 강화 구현 위해 추가한 로직, 안되면 수정(편성 중인 영웅-> selectedHeroes, 클릭한 영웅-> clickedHero)
        clickedHeroIndex = index;
        clickedHeroProfileImg = heroes[index].ProfileImg;
        heroButtons[index].Select();
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
    // 배치한 영웅 슬롯 클릭 시 슬롯에서 제거
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
    // 선택해서 배치 슬롯에 들어간 영웅 전신 이미지 표시
    private void SetMainCharacterImage(Sprite mainSprite)
    {
        CharacterImage.sprite = mainSprite;
        CharacterImage.gameObject.SetActive(mainSprite != null);
    }
    // 리셋 버튼(리셋시 영웅 편성 정보도 날라감)
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
    // 영웅 편성 정보 저장
    private void SaveHeroSelection()
    {
        heroGameManager.SaveHeroFormation();
        _notice.SUB("Save Successefully!");
    }
    
    //  시작할때 영웅 편성 정보 가져오는 메서드
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
    
    // 영웅 클릭한 상태로 강화 버튼 누를 시 선택한 영웅 정보가 HeroUpgrade Scene으로 전달
    private void ReinforceBtnClicked()
    {
        if (clickedHeroIndex != -1)
        {
            HeroData clickedHero = heroes[clickedHeroIndex];
            heroGameManager.SetUpgradeHero(clickedHero);
            SceneManager.LoadScene("HeroUpgrade");
        }
        else
        {
            CustomLogger.Log("No Hero selected for upgrade");
        }
        
    }
    
}
