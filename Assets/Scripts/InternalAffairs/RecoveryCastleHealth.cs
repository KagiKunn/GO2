using System;
using System.IO;
using DefenseScripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RecoveryCastleHealth : MonoBehaviour
{
    private string path;
    private float maxHealth;
    private float crntHealth;
    private float emptyHealth;
    private string json;
    public Slider repairSlider;
    private int repairHealth;
    private int spendGold;
    private TextMeshProUGUI repairText;
    private TextMeshProUGUI goldText;
    private bool repairOK;

    private void Awake()
    {
        crntHealth = PlayerLocalManager.Instance.lCastleHp;
        maxHealth = PlayerLocalManager.Instance.lCastleMaxHp;
        emptyHealth = maxHealth - crntHealth;
        if (repairSlider != null)
        {
            repairSlider.maxValue = emptyHealth / 10;
            repairSlider.value = 0;
        }

        SetRepairPopup();
        SetupButtons();
    }

    private void Update()
    {
        RepairSliderController();
    }

    //골드 미세조정 버튼
    private void SetupButtons()
    {
        // GoldUpButton을 찾고 클릭 이벤트를 추가
        Transform upButtonTransform = gameObject.transform.Find("PopupBackground/GoldUpButton");
        if (upButtonTransform != null)
        {
            Button upButton = upButtonTransform.GetComponent<Button>();
            upButton.onClick.AddListener(IncreaseSliderValue);
        }
        else
        {
            Debug.LogWarning("GoldUpButton not found");
        }

        // GoldDownButton을 찾고 클릭 이벤트를 추가
        Transform downButtonTransform = gameObject.transform.Find("PopupBackground/GoldDownButton");
        if (downButtonTransform != null)
        {
            Button downButton = downButtonTransform.GetComponent<Button>();
            downButton.onClick.AddListener(DecreaseSliderValue);
        }
        else
        {
            Debug.LogWarning("GoldDownButton not found");
        }
    }

    private void IncreaseSliderValue()
    {
        if (repairSlider != null && repairSlider.value < repairSlider.maxValue)
        {
            repairSlider.value += 1;
        }
    }

    private void DecreaseSliderValue()
    {
        if (repairSlider != null && repairSlider.value > repairSlider.minValue)
        {
            repairSlider.value -= 1;
        }
    }

    public void RepairSliderController()
    {
        spendGold = repairSlider != null ? (int)repairSlider.value : 0;
        repairHealth = spendGold * 10;

        // 채워야 하는 체력이 10 미만일 경우 처리
        if (emptyHealth > 0 && emptyHealth < 10)
        {
            repairHealth = (int)emptyHealth;
            spendGold = 1; // 최소 골드 1을 소비하여 나머지 체력을 채움
            repairSlider.value = 1; // 슬라이더 값을 1로 설정
        }
        else if (emptyHealth - repairHealth < 10 && emptyHealth > repairHealth)
        {
            repairHealth = (int)emptyHealth; // 나머지 체력을 모두 채움
            spendGold += 1; // 추가로 1 골드를 소비
            repairSlider.value = spendGold; // 슬라이더 값을 조정
        }

        if (repairText != null)
        {
            repairText.text = $"HP: {repairHealth}";
        }

        if (goldText != null)
        {
            goldText.text = $"GOLD: {spendGold}";
        }
    }

    public void OnRecovery()
    {
        if (PlayerLocalManager.Instance.lMoney < spendGold)
        {
            CustomLogger.Log("그지새끼임", "cyan");
            CallNotEnoughGoldPopup();
            repairOK = false;
            return;
        }

        PlayerLocalManager.Instance.lMoney -= spendGold;
        int repairResult = (int)PlayerLocalManager.Instance.lCastleHp + repairHealth;

        if (Mathf.Approximately(repairResult, PlayerLocalManager.Instance.lCastleMaxHp))
        {
            PlayerLocalManager.Instance.lCastleHp = maxHealth;
        }
        else
        {
            PlayerLocalManager.Instance.lCastleHp = crntHealth + repairHealth;
        }

        repairOK = true;
        PlayerLocalManager.Instance.Save();
    }

    public void SetRepairPopup()
    {
        // if (GameObject.FindWithTag("Popup") != null) return;

        // PopupBackground 패널 하위의 ConfirmButton 찾기
        Transform confirmButtonTransform = gameObject.transform.Find("PopupBackground/ConfirmButton");
        Button confirmButton = confirmButtonTransform.GetComponent<Button>();

        // PopupBackground 패널 하위의 CancelButton 찾기
        Transform cancelButtonTransform = gameObject.transform.Find("PopupBackground/CancelButton");
        Button cancelButton = cancelButtonTransform.GetComponent<Button>();

        Transform repairTextTransform = gameObject.transform.Find("PopupBackground/RepairText");
        repairText = repairTextTransform.GetComponent<TextMeshProUGUI>();

        Transform goldTextTransform = gameObject.transform.Find("PopupBackground/GoldText");
        goldText = goldTextTransform.GetComponent<TextMeshProUGUI>();

        // 확인 버튼 클릭 이벤트 등록
        confirmButton.onClick.AddListener(() =>
        {
            OnRecovery();
            CustomLogger.Log("Confirm button clicked!");
            if (repairOK)
            {
                Destroy(gameObject); // 팝업을 닫음
            }
        });

        // 취소 버튼 클릭 시 팝업을 닫기
        cancelButton.onClick.AddListener(() =>
        {
            CustomLogger.Log("Cancel button clicked!");
            Destroy(gameObject);
        });

        CustomLogger.Log("Event listeners added.");
    }

    public void SetFullPopup()
    {
        if (GameObject.FindWithTag("Popup") != null) return;

        GameObject fullPopup = Instantiate(Resources.Load<GameObject>("PreFab/SmithPopupHpFull"));

        fullPopup.tag = "Popup";

        Transform confirmButtonTransform = fullPopup.transform.Find("PopupBackground/CancelButton");
        Button confirmButton = confirmButtonTransform.GetComponent<Button>();

        confirmButton.onClick.AddListener(() =>
        {
            CustomLogger.Log("Confirm button clicked!");
            Destroy(fullPopup); // 팝업을 닫음
        });
    }

    public void CallNotEnoughGoldPopup()
    {
        GameObject goldPopup = Instantiate(Resources.Load<GameObject>("PreFab/SmithPopupNoGold"));
        Transform confirmButtonTransform = goldPopup.transform.Find("PopupBackground/ConfirmButton");
        Button confirmButton = confirmButtonTransform.GetComponent<Button>();

        confirmButton.onClick.AddListener(() =>
        {
            CustomLogger.Log("Confirm button clicked!");
            Destroy(goldPopup); // 팝업을 닫음
        });
    }
}