using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public AudioClip title;
    public AudioClip ending;
    public AudioClip inter;
    public AudioClip defense;
    public AudioSource audio;
    private string nowSong = "none";

    private BGMManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Title":
            case "RoguePoint":
                if (!nowSong.Equals(title.name))
                {
                    nowSong = title.name;
                    audio.resource = title;
                    audio.Play();
                }

                break;
            case "InternalAffairs":
            case "HeroManagement":
            case "HeroUpgrade":
            case "UnitManagement":
            case "UnitUpgrade":
            case "UnitShop":
                if (!nowSong.Equals(inter.name))
                {
                    nowSong = inter.name;
                    audio.resource = inter;
                    audio.Play();
                }

                break;
            case "Defense":
                if (!nowSong.Equals(defense.name))
                {
                    nowSong = defense.name;
                    audio.resource = defense;
                    audio.Play();
                }

                break;
            case "EndingCredit":
                if (!nowSong.Equals(ending.name))
                {
                    nowSong = ending.name;
                    audio.resource = ending;
                    audio.Play();
                }

                break;
            default:
            {
                nowSong = "none";
                audio.Stop();
            }
                break;
        }
    }
}