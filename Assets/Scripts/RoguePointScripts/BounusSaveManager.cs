using System.IO;
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

    public BounusScriptableObject bounusDataScriptableObject;  // 스크립터블 오브젝트 참조
    public PlayerDataScriptableObject playerDataScriptableObject;

    void Awake()
    {
        filePath = Path.Combine(Application.dataPath, "save", "RogueLike", "Bounus.json");
        saveFilePath = Path.Combine(Application.dataPath, "save", "PlayerData.json");
        CreateDirectoryIfNotExists(filePath);
        CreateDirectoryIfNotExists(saveFilePath);
        
        bounusDataScriptableObject.LoadFromJson(filePath);
        playerDataScriptableObject.LoadFromJson(saveFilePath);
        
        startGold = GameObject.Find("StartGold");
        earnGold = GameObject.Find("EarnGold");
        castleHealth = GameObject.Find("CastleHealth");
        cooldown = GameObject.Find("Cooldown");

        startGold.GetComponent<UIButtonToggle>().isActive = bounusDataScriptableObject.data.startGold1;
        earnGold.GetComponent<UIButtonToggle>().isActive = bounusDataScriptableObject.data.moreEarnGold1;
        castleHealth.GetComponent<UIButtonToggle>().isActive = bounusDataScriptableObject.data.moreCastleHealth1;
        cooldown.GetComponent<UIButtonToggle>().isActive = bounusDataScriptableObject.data.reduceCooldown1;

        saveButton.onClick.AddListener(() => SaveGame());
    }

    public void SaveGame()
    {
        if (bounusDataScriptableObject == null || bounusDataScriptableObject.data == null)
        {
            Debug.LogError("BounusScriptableObject or its data is not assigned.");
            return;
        }

        bounusDataScriptableObject.data.startGold1 = startGold.GetComponent<UIButtonToggle>().isActive;
        bounusDataScriptableObject.data.moreEarnGold1 = earnGold.GetComponent<UIButtonToggle>().isActive;
        bounusDataScriptableObject.data.moreCastleHealth1 = castleHealth.GetComponent<UIButtonToggle>().isActive;
        bounusDataScriptableObject.data.reduceCooldown1 = cooldown.GetComponent<UIButtonToggle>().isActive;

        bounusDataScriptableObject.SaveToJson(filePath);

        Debug.Log("Game Saved: " + JsonUtility.ToJson(bounusDataScriptableObject.data));

        playerDataScriptableObject.data.playerId1 = playerDataScriptableObject.data.playerId1;
        if (bounusDataScriptableObject.data.startGold1)
        {
            playerDataScriptableObject.data.money1 = 500;
        }
        else
        {
            playerDataScriptableObject.data.money1 = 0;
        }
        playerDataScriptableObject.data.soul1 = playerDataScriptableObject.data.soul1;
        
        playerDataScriptableObject.SaveToJson(saveFilePath);
        
        Debug.Log("Player Data Saved: " + JsonUtility.ToJson(playerDataScriptableObject.data));
        
        SceneManager.LoadScene("Defense");
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
