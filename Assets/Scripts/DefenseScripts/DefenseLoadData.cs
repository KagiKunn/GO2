using System.IO;
using UnityEngine;

// 이 스크립트에서는 로드만 할것
public class DefenseLoadData : MonoBehaviour
{
    private string filePath;
    
    private GameObject startGold;
    private GameObject earnGold;
    private GameObject castleHealth;
    private GameObject cooldown;
    void Awake()
    {
        filePath = Path.Combine(Application.dataPath, "save", "RogueLike", "Bounus.json");
        // 여기서 게임 세이브파일 로드
        
        // 여기부턴 로그라이크 포인트 로드
        gameObject.GetComponent<DefenseInit>().startGold1 = LoadBounus().startGold;
        gameObject.GetComponent<DefenseInit>().earnGold1 = LoadBounus().moreEarnGold;
        gameObject.GetComponent<DefenseInit>().castleHealth1 = LoadBounus().moreCastleHealth;
        gameObject.GetComponent<DefenseInit>().cooldown1 = LoadBounus().reduceCooldown;
    }

    void Update()
    {
        
        
    }
    public Bounus LoadBounus()
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
