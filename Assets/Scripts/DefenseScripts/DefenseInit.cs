using System;
using UnityEngine;

public class DefenseInit : MonoBehaviour
{
    private bool startGold;
    private bool earnGold;
    private bool castleHealth;
    private bool cooldown;

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

    private void Awake()
    {
        if (earnGold)
        {
            //적 죽일때 골드량 증가 로직
        }

        if (castleHealth)
        {
            GameObject Wall = GameObject.Find("Wall HP Controller");
            Wall.GetComponent<CastleWallManager>().extraHealth1 = 500f;
        }
    }
}
