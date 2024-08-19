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
        startGold = GameObject.Find("StartGold");
        earnGold = GameObject.Find("EarnGold");
        castleHealth = GameObject.Find("CastleHealth");
        cooldown = GameObject.Find("Cooldown");

        startGold.GetComponent<UIButtonToggle>().level = PlayerLocalManager.Instance.lStartGold;
        earnGold.GetComponent<UIButtonToggle>().level = PlayerLocalManager.Instance.lMoreEarnGold;
        castleHealth.GetComponent<UIButtonToggle>().level = PlayerLocalManager.Instance.lMoreCastleHealth;
        cooldown.GetComponent<UIButtonToggle>().level = PlayerLocalManager.Instance.lReduceCooldown;

        soul = PlayerSyncManager.Instance.RoguePoint != PlayerLocalManager.Instance.lPoint
            ? PlayerLocalManager.Instance.lPoint
            : PlayerSyncManager.Instance.RoguePoint;
        CustomLogger.Log(soul, "yellow");
        GameObject.Find("Soul").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = soul.ToString();
        saveButton.onClick.AddListener(() => SaveGame());
    }

    public void SaveGame()
    {
        PlayerLocalManager.Instance.lStartGold = startGold.GetComponent<UIButtonToggle>().level;
        PlayerLocalManager.Instance.lMoreEarnGold = earnGold.GetComponent<UIButtonToggle>().level;
        PlayerLocalManager.Instance.lMoreCastleHealth = castleHealth.GetComponent<UIButtonToggle>().level;
        PlayerLocalManager.Instance.lReduceCooldown = cooldown.GetComponent<UIButtonToggle>().level;
        PlayerLocalManager.Instance.lPoint =
            int.Parse(GameObject.Find("Soul").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
        PlayerLocalManager.Instance.Save();

        StartGoldSetup();

        SceneManager.LoadScene("InternalAffairs");
    }
    
    void StartGoldSetup()
    {
        PlayerLocalManager.Instance.lMoney = PlayerLocalManager.Instance.lStartGold switch
        {
            1 => 100,
            2 => 200,
            3 => 300,
            4 => 400,
            _ => 0
        };
    }

}