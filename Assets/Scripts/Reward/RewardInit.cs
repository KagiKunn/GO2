using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RewardInit : MonoBehaviour
{
    private Animator animator;
    private int moneyReward;

    [SerializeField] private GameObject chestBtn;
    [SerializeField] private GameObject chest;
    [SerializeField] private GameObject coin;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = chest.GetComponent<Animator>();
        coin.SetActive(false);
        chest.SetActive(false);
        moneyReward = Random.Range(250, 501);
        CustomLogger.Log(moneyReward);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenChest()
    {
        chestBtn.SetActive(false);
        chest.SetActive(true);
        CustomLogger.Log("Chest opened!");
        animator.SetTrigger("ChestOpen");
        PlayerLocalManager.Instance.lMoney += moneyReward;
    }

    public void NextScene()
    {
        SceneManager.LoadScene("InternalAffairs");
    }

    public void ShowGold()
    {
        coin.SetActive(true);
        GameObject.Find("GoldReward").GetComponent<TextMeshProUGUI>().text = moneyReward.ToString();
    }   
}

