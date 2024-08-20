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
        gameObject.GetComponent<DefenseInit>().startGold1 = PlayerLocalManager.Instance.lStartGold;
        gameObject.GetComponent<DefenseInit>().earnGold1 = PlayerLocalManager.Instance.lMoreEarnGold;
        gameObject.GetComponent<DefenseInit>().castleHealth1 = PlayerLocalManager.Instance.lMoreCastleHealth;
        gameObject.GetComponent<DefenseInit>().cooldown1 = PlayerLocalManager.Instance.lReduceCooldown;
    }

    void Update()
    {
        
        
    }
}
