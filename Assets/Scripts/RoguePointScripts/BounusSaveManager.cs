using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BounusSaveManager : MonoBehaviour
{
    private string filePath;
    private string saveFilePath;

    public Button saveButton;  // 저장 버튼
    private GameObject startGold;
    private GameObject earnGold;
    private GameObject castleHealth;
    private GameObject cooldown;

    public int soul;
    public BounusScriptableObject bounusDataScriptableObject;  // 스크립터블 오브젝트 참조
    public PlayerDataScriptableObject playerDataScriptableObject;
    // public GameObject soulObject;
    void Awake()
    {
        filePath = Path.Combine(Application.dataPath, "save", "RogueLike", "Bounus.json");
        CreateDirectoryIfNotExists(filePath);
        bounusDataScriptableObject.LoadFromJson(filePath);
        
        saveFilePath = Path.Combine(Application.dataPath, "save", "PlayerData.json");
        CreateDirectoryIfNotExists(saveFilePath);
        playerDataScriptableObject.LoadFromJson(saveFilePath);
        
        startGold = GameObject.Find("StartGold");
        earnGold = GameObject.Find("EarnGold");
        castleHealth = GameObject.Find("CastleHealth");
        cooldown = GameObject.Find("Cooldown");

        startGold.GetComponent<UIButtonToggle>().level = bounusDataScriptableObject.data.startGold1;
        earnGold.GetComponent<UIButtonToggle>().level = bounusDataScriptableObject.data.moreEarnGold1;
        castleHealth.GetComponent<UIButtonToggle>().level = bounusDataScriptableObject.data.moreCastleHealth1;
        cooldown.GetComponent<UIButtonToggle>().level = bounusDataScriptableObject.data.reduceCooldown1;

        soul = playerDataScriptableObject.data.soul1;
        CustomLogger.Log(soul,"yellow");
        GameObject.Find("Soul").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = soul.ToString();
        saveButton.onClick.AddListener(() => SaveGame());
    }

    public void SaveGame()
    {
        if (bounusDataScriptableObject == null || bounusDataScriptableObject.data == null)
        {
            Debug.LogError("BounusScriptableObject or its data is not assigned.");
            return;
        }

        bounusDataScriptableObject.data.startGold1 = startGold.GetComponent<UIButtonToggle>().level;
        bounusDataScriptableObject.data.moreEarnGold1 = earnGold.GetComponent<UIButtonToggle>().level;
        bounusDataScriptableObject.data.moreCastleHealth1 = castleHealth.GetComponent<UIButtonToggle>().level;
        bounusDataScriptableObject.data.reduceCooldown1 = cooldown.GetComponent<UIButtonToggle>().level;

        bounusDataScriptableObject.SaveToJson(filePath);

        Debug.Log("Game Saved: " + JsonUtility.ToJson(bounusDataScriptableObject.data));

        playerDataScriptableObject.data.playerId1 = playerDataScriptableObject.data.playerId1;
        if (bounusDataScriptableObject.data.startGold1>0)
        {
            playerDataScriptableObject.data.money1 = 500;
        }
        else
        {
            playerDataScriptableObject.data.money1 = 0;
        }

        playerDataScriptableObject.data.soul1 = int.Parse(GameObject.Find("Soul").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
        
        playerDataScriptableObject.SaveToJson(saveFilePath);
        
        Debug.Log("Player Data Saved: " + JsonUtility.ToJson(playerDataScriptableObject.data));
        
        SceneManager.LoadScene("InternalAffairs");
    }

    private void CreateDirectoryIfNotExists(string path)
    {
        string directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            Debug.Log("Directory created at: " + directory);
        }
    }
}
