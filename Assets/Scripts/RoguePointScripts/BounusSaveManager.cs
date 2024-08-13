using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BounusSaveManager : MonoBehaviour
{
    private string filePath;

    public Button saveButton;  // 저장 버튼
    private GameObject startGold;
    private GameObject earnGold;
    private GameObject castleHealth;
    private GameObject cooldown;
    void Awake()
    {
        
        filePath = Path.Combine(Application.dataPath, "save", "RogueLike", "Bounus.json");
        // 파일 저장 경로에 디렉터리가 없으면 생성
        CreateDirectoryIfNotExists(filePath);
        
        startGold = GameObject.Find("StartGold");
        earnGold = GameObject.Find("EarnGold");
        castleHealth = GameObject.Find("CastleHealth");
        cooldown = GameObject.Find("Cooldown");
        startGold.GetComponent<UIButtonToggle>().isActive = LoadGame().startGold;
        earnGold.GetComponent<UIButtonToggle>().isActive = LoadGame().moreEarnGold;
        castleHealth.GetComponent<UIButtonToggle>().isActive = LoadGame().moreCastleHealth;
        cooldown.GetComponent<UIButtonToggle>().isActive = LoadGame().reduceCooldown;

        // 버튼에 클릭 이벤트 리스너 추가
        saveButton.onClick.AddListener(() => SaveGame());
    }

    public void SaveGame()
    {
        Bounus state = new Bounus();
        state.startGold = startGold.GetComponent<UIButtonToggle>().isActive;
        state.moreEarnGold = earnGold.GetComponent<UIButtonToggle>().isActive;
        state.moreCastleHealth = castleHealth.GetComponent<UIButtonToggle>().isActive;
        state.reduceCooldown = cooldown.GetComponent<UIButtonToggle>().isActive;
        string json = JsonUtility.ToJson(state);
        File.WriteAllText(filePath, json);

        Debug.Log("Game Saved: " + json);
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

    public Bounus LoadGame()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Bounus state = JsonUtility.FromJson<Bounus>(json);
            Debug.Log("Game Loaded: " + json);
            return state;
        }
    
        Debug.Log("No save file found, returning default state.");
        return new Bounus();
    }
}