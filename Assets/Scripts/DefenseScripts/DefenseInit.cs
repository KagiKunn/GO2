using System;
using UnityEngine;

public class DefenseInit : MonoBehaviour
{
    private bool startGold;
    private bool earnGold;
    private bool castleHealth;
    private bool cooldown;

    public int currentGold;
    public int extraGold;
    public bool startGold1
    {
        get => startGold;
        set => startGold = value;
    }

    public bool earnGold1
    {
        get => earnGold;
        set => earnGold = value;
    }

    public bool castleHealth1
    {
        get => castleHealth;
        set => castleHealth = value;
    }

    public bool cooldown1
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
    
    private void Awake()
    {
        if (earnGold)
        {
            extraGold1 = 10;
        }
        else
        {
            extraGold1 = 0;
        }

        if (castleHealth)
        {
            GameObject Wall = GameObject.Find("Wall HP Controller");
            CustomLogger.Log("여기"+Wall,"blue");
            Wall.GetComponent<CastleWallManager>().extraHealth1 = 500f;
        }

        PlayerDataManager playerDataManager = gameObject.GetComponent<PlayerDataManager>();
        if (playerDataManager != null)
        {
            currentGold = playerDataManager.GetMoney();
            Debug.Log("Current Money: " + currentGold);
        }
        else
        {
            Debug.LogWarning("PlayerDataManager not found in the scene.");
        }
    }
}
