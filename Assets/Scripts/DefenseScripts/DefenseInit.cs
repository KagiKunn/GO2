using System;
using UnityEngine;

public class DefenseInit : MonoBehaviour
{
    private int startGold;
    private int earnGold;
    private int castleHealth;
    private int cooldown;

    public int currentGold;
    public int extraGold;
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
    
    private void Awake()
    {
        if (earnGold>0)
        {
            extraGold1 = 10;
        }
        else
        {
            extraGold1 = 0;
        }

        if (castleHealth>0)
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
