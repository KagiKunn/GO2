using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class RewardInit : MonoBehaviour
{
    private Animator animator;
    private int moneyReward;
    public AudioClip soundClip;
    private AudioSource attackAudioSource;
    public AudioMixerGroup sfxMixerGroup;
    [SerializeField] private GameObject chestBtn;
    [SerializeField] private GameObject chest;
    [SerializeField] private GameObject coin;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackAudioSource = gameObject.AddComponent<AudioSource>();
        attackAudioSource.clip = soundClip;
        attackAudioSource.outputAudioMixerGroup = sfxMixerGroup;
        animator = chest.GetComponent<Animator>();
        coin.SetActive(false);
        chest.SetActive(false);
        moneyReward = Random.Range(250, 501);
        CustomLogger.Log(moneyReward);
    }
    public void PlaySound()
    {
        if (attackAudioSource != null && soundClip != null)
        {
            attackAudioSource.Play();
        }
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
        PlaySound();
        coin.SetActive(true);
        GameObject.Find("GoldReward").GetComponent<TextMeshProUGUI>().text = moneyReward.ToString();
    }   
}

