using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BounusSaveManager : MonoBehaviour
{
    private string filePath;
    private string saveFilePath;

    public Button saveButton; // 저장 버튼
    private GameObject startGold;
    private GameObject earnGold;
    private GameObject castleHealth;
    private GameObject cooldown;

    public int soul;

    void Awake()
    {
        if (PlayerLocalManager.Instance.lGameStarted)
        {
            SceneManager.LoadScene("InternalAffairs");
        }
        startGold = GameObject.Find("StartGold");
        earnGold = GameObject.Find("EarnGold");
        castleHealth = GameObject.Find("CastleHealth");
        cooldown = GameObject.Find("Cooldown");

        startGold.GetComponent<UIButtonToggle>().level = PlayerLocalManager.Instance.lStartGold;
        earnGold.GetComponent<UIButtonToggle>().level = PlayerLocalManager.Instance.lMoreEarnGold;
        castleHealth.GetComponent<UIButtonToggle>().level = PlayerLocalManager.Instance.lMoreCastleHealth;
        cooldown.GetComponent<UIButtonToggle>().level = PlayerLocalManager.Instance.lReduceCooldown;
        
        bool allZero = PlayerLocalManager.Instance.lStartGold == 0 &&
                       PlayerLocalManager.Instance.lMoreEarnGold == 0 &&
                       PlayerLocalManager.Instance.lMoreCastleHealth == 0 &&
                       PlayerLocalManager.Instance.lReduceCooldown == 0;

        if (PlayerSyncManager.Instance != null && PlayerSyncManager.Instance.isOnline)
        {
            soul = (allZero && PlayerSyncManager.Instance.RoguePoint != PlayerLocalManager.Instance.lPoint) 
                ? PlayerSyncManager.Instance.RoguePoint 
                : PlayerLocalManager.Instance.lPoint;
        }
        else
        {
            soul = PlayerLocalManager.Instance.lPoint;
        }
        CustomLogger.Log(soul, "yellow");
        GameObject.Find("Soul").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = soul.ToString();
        saveButton.onClick.AddListener(() => SaveGame());
    }

    public void SaveGame()
    {
        PlayerLocalManager.Instance.lMoney += PlayerLocalManager.Instance.lStartGold;
        PlayerLocalManager.Instance.lStartGold = startGold.GetComponent<UIButtonToggle>().level;
        PlayerLocalManager.Instance.lMoreEarnGold = earnGold.GetComponent<UIButtonToggle>().level;
        PlayerLocalManager.Instance.lMoreCastleHealth = castleHealth.GetComponent<UIButtonToggle>().level;
        PlayerLocalManager.Instance.lReduceCooldown = cooldown.GetComponent<UIButtonToggle>().level;
        PlayerLocalManager.Instance.lPoint =
            int.Parse(GameObject.Find("Soul").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
        PlayerLocalManager.Instance.lGameStarted = true;
        StartGoldSetup();
        StartHealthSetup();
        PlayerLocalManager.Instance.Save();
        
        SceneManager.LoadScene("InternalAffairs");
    }

    public void Back()
    {
        SceneManager.LoadScene("Title");
    }
    void StartGoldSetup()
    {
        PlayerLocalManager.Instance.lMoney = PlayerLocalManager.Instance.lStartGold switch
        {
            1 => 1500,
            2 => 2000,
            3 => 2500,
            4 => 3000,
            _ => 1000
        };
    }

    void StartHealthSetup()
    {
        PlayerLocalManager.Instance.lCastleExtraHp = PlayerLocalManager.Instance.lMoreCastleHealth switch
        {
            1 => 500,
            2 => 1000,
            3 => 1500,
            4 => 2000,
            _ => 0
        };
        PlayerLocalManager.Instance.lCastleMaxHp += PlayerLocalManager.Instance.lCastleExtraHp;
        PlayerLocalManager.Instance.lCastleHp = PlayerLocalManager.Instance.lCastleMaxHp;
    }

}