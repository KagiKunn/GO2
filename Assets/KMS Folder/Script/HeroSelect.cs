using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelect : MonoBehaviour
{
    // 영웅창에 있는 영웅 요소들
    public Button[] heroes;
    private int selectedHeroIndex = -1;
    // 캐릭터 전신 이미지, 현재 팀 편성 저장, 편성 리셋 버튼
    public Image CharacterImage;
    // 편성되어있는 영웅 요소들
    public Button[] selectedHeroes;
    // 아무 영웅도 선택되지 않은 상태
    public Button resetBtn, saveBtn;
    // MainHero객체
    public GameObject MainHero;
    
    private void Start()
    {
        // 영웅 버튼 클릭 리스너 할당
        for (int i = 0; i < heroes.Length ; i++)
        {
            int index = i; // 클로저
            heroes[i].onClick.AddListener(() => OnHeroesClicked(index));
        }
        // 배치된 영웅 이미지 슬롯 클릭 리스터 할당
        for (int i = 0; i <selectedHeroes.Length ; i++)
        {
            int index = i;
            selectedHeroes[i].onClick.AddListener(()=>OnSelectedHeroClicked(index));
        }
        // reset, save 버튼에 버튼 클릭 리스너 할당
        resetBtn.onClick.AddListener(OnResetBtnClicked);
        saveBtn.onClick.AddListener(OnSaveBtnClicked);
        // save(영웅 편성) 정보 불러오기
        LoadHeroFormation();

    }
    public void OnHeroesClicked(int index)
    {
        CustomLogger.Log("Heroes Button Clicked");
        selectedHeroIndex = index;
        Sprite heroSprite = GetHeroMainImage(index); // 영웅의 메인 이미지 추출을 위한 메서드(누끼 이미지)
        
        // 이미 편성되어 있는 영웅인지 확인
        for (int i = 0; i < selectedHeroes.Length; i++)
        {   //  편성되어있을 시 못넣게 설정
            if (selectedHeroes[i].GetComponent<Image>().sprite == heroes[index].GetComponent<Image>().sprite)
            {
                return;
            }
        }
        // 영웅 편성하기
        for (int i = 0; i < selectedHeroes.Length; i++)
        {
            Image selectedHero = selectedHeroes[i].GetComponent<Image>(); // 선택된 영웅 이미지
            if (selectedHero.sprite == null)
            {
                selectedHero.sprite = heroes[index].GetComponent<Image>().sprite;
                selectedHero.enabled = true; 
                SetMainCharacterImage(index); // 선택된 후 바로 CharacterImage 설정
                break;
            }
        }
        selectedHeroIndex = -1;
    }

    public void OnSelectedHeroClicked(int index)
    {
        CustomLogger.Log("Selected Heroes Button Clicked");
        // 하단 이미지 슬롯을 클릭했을 때 해당 슬롯의 이미지를 제거
        Image selectedHero = selectedHeroes[index].GetComponent<Image>();
        selectedHero.sprite = null;
        selectedHero.enabled = false;
    }

    public void SetMainCharacterImage(int heroIndex = -1)
    {
        CustomLogger.Log("Main Image", "blue");

        if (heroIndex >=0)
        {
            CharacterImage.sprite = GetHeroMainImage(heroIndex);
            MainHero.SetActive(true);
        }
        
        else if (selectedHeroes[0].GetComponent<Image>().sprite != null)
        {
            // 선택된 영웅의 전신 이미지를 CharacterImage에 설정하고 MainHero 활성화
            int firstHeroIndex = GetHeroIndex(selectedHeroes[0].GetComponent<Image>().sprite);
            CharacterImage.sprite = GetHeroMainImage(firstHeroIndex);
            MainHero.SetActive(true);
        }
        else
        {
            CharacterImage.sprite = null;
            MainHero.SetActive(false);
        }
    }

    private Sprite GetHeroMainImage(int index)
    {
        return heroes[index].transform.Find("MainImage")?.GetComponent<SpriteRenderer>()?.sprite;
    }

    private int GetHeroIndex(Sprite sprite)
    {
        for (int i = 0; i < heroes.Length; i++)
        {
            if (heroes[i].GetComponent<Image>().sprite == sprite)
            {
                return i;
            }
        }
        return -1;
    }
    
    public void OnResetBtnClicked()
    {
        for (int i = 0; i < selectedHeroes.Length; i++)
        {
            Image selectedHero = selectedHeroes[i].GetComponent<Image>();
            selectedHero.sprite = null;
            selectedHero.enabled = false;
        }
        SetMainCharacterImage();
    }

    public void OnSaveBtnClicked()
    {
        
    }

    public void LoadHeroFormation()
    {
        
    }
    
    
}
