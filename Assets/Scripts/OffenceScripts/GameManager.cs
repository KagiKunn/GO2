using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#pragma warning disable CS0108, CS0114

#pragma warning disable CS0414, CS0618 // 필드가 대입되었으나 값이 사용되지 않습니다

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [Header("# Game Control")] [SerializeField]
    private bool isLive;

    [SerializeField] private float gameTime;
    [SerializeField] private float maxGameTime = 2 * 10f;

    [Header("# Player Info")] [SerializeField]
    private int playerId;

    [SerializeField] private float health;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private int level;
    [SerializeField] private int kill;
    [SerializeField] private int exp;
    [SerializeField] private int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };

    [Header("# Game Object")] [SerializeField]
    private Player[] players;

    [SerializeField] private PoolManager poolManager;
    [SerializeField] private LevelUp uiLevelUp;
    [SerializeField] private Result uiResult;
    [SerializeField] private GameObject enemyCleaner;
    [SerializeField] private CinemachineCamera camera;

    private List<string> selectedHeroes = new List<string>();
    private string filePath;

    private Hand hand;
    private Weapon weapon;

    public enum HeroNames
    {
        Dummy,
        KKS01,
        Novice,
        Knight,
        DualWeapon,
        Elf,
        LSH01,
        CHS01
    }

    public HeroNames heroNames;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

        List<HeroData> heroList = HeroManager.Instance.selectedHeroes;
        foreach (var hero in heroList.Where(hero => hero != null))
        {
            selectedHeroes.Add(hero.Name);
        }

        // foreach (string selectedHero in selectedHeroes) {
        // 	CustomLogger.Log(selectedHero);
        // }
    }

    public void GameStart(Text text)
    {
        health = maxHealth;

        foreach (var t in selectedHeroes)
        {
            CustomLogger.Log(text.text);

            if (text.text == t)
            {
                if (Enum.TryParse(t, out HeroNames heroEnum))
                {
                    players[(int)heroEnum].gameObject.SetActive(true);
                    playerId = (int)heroEnum;

                    camera.Target.TrackingTarget = players[(int)heroEnum].gameObject.transform;
                }
                else
                {
                    CustomLogger.Log("Invalid hero name: " + t);
                }
            }
        }

        uiLevelUp.Select(playerId + 5);
        Resume();
        AudioManager.Instance.PlayBgm(true);
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Lose();

        Stop();

        AudioManager.Instance.PlayBgm(false);
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Lose);
    }

    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        enemyCleaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Win();

        Stop();

        AudioManager.Instance.PlayBgm(false);
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Win);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene("InternalAffairs");
    }

    public void GameWin()
    {
        SceneManager.LoadScene("Gatcha");
    }

    private void Update()
    {
        if (!isLive) return;

        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;

            GameVictory();
        }
    }

    private void Initialize()
    {
        if (players == null || players.Length == 0)
        {
            players = FindObjectsOfType<Player>();

            if (players == null || players.Length == 0)
            {
                CustomLogger.Log("Player instances not found", "red");
            }
            else
            {
                CustomLogger.Log("Player instances found and assigned");
            }
        }

        if (poolManager == null)
        {
            poolManager = FindObjectOfType<PoolManager>();

            if (poolManager == null)
            {
                CustomLogger.Log("PoolManager instance not found", "red");
            }
            else
            {
                CustomLogger.Log("PoolManager instance found and assigned");
            }
        }
    }

    public void GetExp()
    {
        if (!isLive) return;

        exp++;

        if (exp == nextExp[Mathf.Min(level, nextExp.Length - 1)])
        {
            level++;
            exp = 0;
            health += 10;
            maxHealth += 10;

            uiLevelUp.Show(playerId + 5);
        }
    }

    public static GameManager Instance
    {
        get
        {
            if (instance.IsUnityNull())
            {
                instance = FindObjectOfType<GameManager>();

                if (instance.IsUnityNull())
                {
                    CustomLogger.Log("No Singleton Object", "red");

                    return null;
                }
                else
                {
                    instance.Initialize();
                }
            }

            return instance;
        }
    }

    public void Stop()
    {
        isLive = false;

        Time.timeScale = 0;
    }

    public void Resume()
    {
        isLive = true;

        Time.timeScale = 1;
    }

    public Player[] Player => players;
    public PoolManager PoolManager => poolManager;
    public float GameTime => gameTime;
    public float MaxGameTime => maxGameTime;
    public bool IsLive => isLive;

    public float Health
    {
        get => health;

        set => health = value;
    }

    public float MaxHealth
    {
        get => maxHealth;

        set => maxHealth = value;
    }

    public int Level => level;

    public int Kill
    {
        get => kill;

        set => kill = value;
    }

    public int Exp
    {
        get => exp;

        set => exp = value;
    }

    public int[] NextExp => nextExp;

    public int PlayerId => playerId;
}