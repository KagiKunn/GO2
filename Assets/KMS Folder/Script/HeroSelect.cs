using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HeroSelect : MonoBehaviour 
{
	public NoticeUI _notice;
    public HeroManager heroManager;
	public Image CharacterImage;
	public Button[] heroButtons;
	public Button[] selectedHeroSlots;
	public Button resetBtn, saveBtn, reinforceBtn;
    public LocalizedString saveString;

    private List<HeroData> heroes;
    
    // UpgradeHero씬으로 히어로 정보 들고가기 위해
    private int clickedHeroIndex = -1;
    private Sprite clickedHeroProfileImg;
    // 편성된 영웅 -> selectedHero
    // 클릭한 영웅(강화하려고) -> clickedHero

    void Awake()
    {
        _notice = FindFirstObjectByType<NoticeUI>();
    }
    
    private void Start()
    {
        heroes = HeroManager.Instance.GetHeroes(); 
        InitializeHeroButtons();
        InitializeSelectedHeroSlots();

        heroManager = HeroManager.Instance;
        
		resetBtn.onClick.AddListener(ResetHeroSelection);
		saveBtn.onClick.AddListener(SaveHeroSelection);
        reinforceBtn.onClick.AddListener(ReinforceBtnClicked);

        saveString.TableReference = "UI";
        saveString.TableEntryReference = "SaveSuccess";
        LoadHeroFormation();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("InternalAffairs");
        }
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
        HeroData selectedHeroData = heroes[index];

        // 클릭한 영웅이 이미 편성된 영웅인지 확인
        bool isHeroAlreadySelected = HeroManager.Instance.GetSelectedHeroes().Exists(h => h.Name == selectedHeroData.Name);

        // 영웅 슬롯이 가득 찼거나, 이미 편성된 영웅일 경우 추가하지 않음
        if (!isHeroAlreadySelected && HeroManager.Instance.GetSelectedHeroes().Count < 3)
        {
            AddHeroToSlot(selectedHeroData);
        }
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
                HeroManager.Instance.AddSelectedHero(heroData);
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
            HeroManager.Instance.GetSelectedHeroes().RemoveAll(h => h.ProfileImg == slotImage.sprite);
            slotImage.sprite = null;
            slotImage.enabled = false;
        }
    }
    // 선택해서 배치 슬롯에 들어간 영웅 전신 이미지 표시
    private void SetMainCharacterImage(Sprite mainSprite)
    {
        if (mainSprite != null)
        {
            CharacterImage.sprite = mainSprite;
            CharacterImage.gameObject.SetActive(mainSprite != null);
            CharacterImage.GetComponent<Image>().enabled = true;
            RectTransform rectTransform = CharacterImage.GetComponent<RectTransform>();
            if (CharacterImage.sprite.name.Equals("ElfNuki_0"))
            {
                rectTransform.anchoredPosition = new Vector2(-464, -412);
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(-464, -637);
            }
        }
        else
        {
            Debug.LogWarning("Main sprite is Null");
        }
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
        HeroManager.Instance.ClearHeroFormation();
        SetMainCharacterImage(null);
        clickedHeroIndex = -1; // 리셋 시 클릭된 영웅 인덱스 초기화
        clickedHeroProfileImg = null;
    }
    // 영웅 편성 정보 저장
    private void SaveHeroSelection()
    {
        HeroManager.Instance.SaveHeroFormation();
        saveString.StringChanged += OnSaveSuccessMessageChanged;
        //_notice.SUB("Save Successefully!");
    }
    
    //  시작할때 영웅 편성 정보 가져오는 메서드
    private void LoadHeroFormation()
    {
        heroManager.LoadHeroFormation();
        List<HeroData> selectedHeroes = heroManager.GetSelectedHeroes();
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
        clickedHeroIndex = -1;
        clickedHeroProfileImg = null;
    }
    
    // 영웅 클릭한 상태로 강화 버튼 누를 시 선택한 영웅 정보가 HeroUpgrade Scene으로 전달
    private void ReinforceBtnClicked()
    {
        if (clickedHeroIndex != -1)
        {
            HeroData clickedHero = heroes[clickedHeroIndex];
            HeroManager.Instance.SetUpgradeHero(clickedHero); 
            CustomLogger.Log("Upgrade Hero Set: " + clickedHero.Name);
            SceneManager.LoadScene("HeroUpgrade");
        }
        else
        {
            CustomLogger.Log("No Hero selected for upgrade");
        }
        
    }

    private void OnSaveSuccessMessageChanged(string localizedText)
    {
        _notice.SUB(localizedText);
        saveString.StringChanged -= OnSaveSuccessMessageChanged;
    }
    
}
