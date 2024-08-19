using System;
using UnityEngine;

public class DefenseInit : MonoBehaviour
{
    public AudioClip soundClip;
    private AudioSource audioSource;
    
    
    
    private int startGold;
    private int earnGold;
    private int castleHealth;
    private int cooldown;

    public int currentGold;
    public int extraGold;
    public int extraCool;
    public int startGold1
    {
        get => startGold;
        set => startGold = value;
    }

    public int earnGold1
    {
        get => earnGold;
        set => earnGold = value;
    }

    public int castleHealth1
    {
        get => castleHealth;
        set => castleHealth = value;
    }

    public int cooldown1
    {
        get => cooldown;
        set => cooldown = value;
    }

    public int currentGold1
    {
        get => currentGold;
        set => currentGold = value;
    }

    public int extraGold1
    {
        get => extraGold;
        set => extraGold = value;
    }

    public int extraCool1
    {
        get => extraCool;
        set => extraCool = value;
    }

    private void Awake()
    {
        EarnGoldSetup();

        CastleHealthSetup();
    }

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = soundClip;
        audioSource.loop = true;
        PlaySound();
    }

    public void PlaySound()
    {
        if (audioSource != null && soundClip != null)
        {
            audioSource.Play();
        }
    }
    void EarnGoldSetup()
    {
        switch (earnGold)
        {
            case 1:
                extraGold1 = 10;
                break;
            case 2:
                extraGold1 = 20;
                break;
            case 3:
                extraGold1 = 30;
                break;
            case 4:
                extraGold1 = 40;
                break;
            default:
                extraGold1 = 0;
                break;
        }

    }

    void CastleHealthSetup()
    {
        GameObject Wall = GameObject.Find("Wall HP Controller");
        switch (earnGold)
        {
            case 1:
                Wall.GetComponent<CastleWallManager>().extraHealth1 = 100f;
                break;
            case 2:
                Wall.GetComponent<CastleWallManager>().extraHealth1 = 200f;
                break;
            case 3:
                Wall.GetComponent<CastleWallManager>().extraHealth1 = 300f;
                break;
            case 4:
                Wall.GetComponent<CastleWallManager>().extraHealth1 = 400f;
                break;
            default:
                Wall.GetComponent<CastleWallManager>().extraHealth1 = 0f;
                break;
        }
    }

    void CoolDownSetup()
    {
        switch (cooldown)
        {
            case 1:
                extraCool = 10;
                break;
            case 2:
                extraCool = 20;
                break;
            case 3:
                extraCool = 30;
                break;
            case 4:extraCool = 40;
                break;
            default:
                extraCool = 0;
                break;
        }
    }
}
