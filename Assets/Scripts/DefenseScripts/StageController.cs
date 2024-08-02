using System.Collections.Generic;
using DefenseScripts;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [System.Serializable]
    public class StageEnemies
    {
        public List<GameObject> darkElf;
        public List<GameObject> human;
        public List<GameObject> orc;
        public List<GameObject> skeleton;
        public List<GameObject> witch;
    }

    // 각 스테이지의 적 프리팹 리스트
    [SerializeField] private List<StageEnemies> stageEnemies;
    [SerializeField] private int currentStage = 1;
    [SerializeField] private int totalWave = 3; // 기본값으로 3 웨이브 설정

    private int currentWave = 0;
    private List<int> usedStages = new List<int>();
    private List<string> usedRaces = new List<string>();
    private List<GameObject> enemyPrefabs;
    private GameObject bossEnemy;

    public int CurrentStage
    {
        get { return currentStage; }
    }

    public int TotalWave
    {
        get { return totalWave; }
    }

    public int CurrentWave
    {
        get { return currentWave; }
    }

    public List<GameObject> EnemyPrefabs
    {
        get { return enemyPrefabs; }
    }

    public GameObject BossEnemy
    {
        get { return bossEnemy; }
    }

    public void IncrementStage()
    {
        currentStage++;
        currentWave = 0;
        CustomLogger.Log("IncrementStage: CurrentStage = " + currentStage + ", CurrentWave reset to 0", "blue");
    }

    // 웨이브 카운트 증가
    public void IncrementWave()
    {
        currentWave++;
        CustomLogger.Log("IncrementWave: CurrentWave = " + currentWave, "blue");
    }

    public void ResetWave()
    {
        currentWave = 0;
        CustomLogger.Log("ResetWave: CurrentWave reset to 0", "blue");
    }

    public void SelectRandomEnemyPrefabs()
    {
        List<GameObject> selectedEnemies = new List<GameObject>();
        List<string> availableRaces = new List<string> { "darkElf", "human", "orc", "skeleton", "witch" };
        availableRaces.RemoveAll(race => usedRaces.Contains(race));

        if (availableRaces.Count == 0)
        {
            CustomLogger.Log("모든 종족이 이미 사용되었습니다.", "red");
            enemyPrefabs = selectedEnemies;
            bossEnemy = null;
            return;
        }

        string selectedRace = availableRaces[Random.Range(0, availableRaces.Count)];
        usedRaces.Add(selectedRace);
        CustomLogger.Log("선택된 종족 : " + selectedRace, "yellow");

        StageEnemies stage = stageEnemies[currentStage - 1];

        switch (selectedRace)
        {
            case "darkElf":
                selectedEnemies.AddRange(stage.darkElf);
                if (selectedEnemies.Count > 0)
                {
                    bossEnemy = selectedEnemies[selectedEnemies.Count - 1];
                    selectedEnemies.RemoveAt(selectedEnemies.Count - 1);
                }
                break;
            case "human":
                selectedEnemies.AddRange(stage.human);
                if (selectedEnemies.Count > 0)
                {
                    bossEnemy = selectedEnemies[selectedEnemies.Count - 1];
                    selectedEnemies.RemoveAt(selectedEnemies.Count - 1);
                }
                break;
            case "orc":
                selectedEnemies.AddRange(stage.orc);
                if (selectedEnemies.Count > 0)
                {
                    bossEnemy = selectedEnemies[selectedEnemies.Count - 1];
                    selectedEnemies.RemoveAt(selectedEnemies.Count - 1);
                }
                break;
            case "skeleton":
                selectedEnemies.AddRange(stage.skeleton);
                if (selectedEnemies.Count > 0)
                {
                    bossEnemy = selectedEnemies[selectedEnemies.Count - 1];
                    selectedEnemies.RemoveAt(selectedEnemies.Count - 1);
                }
                break;
            case "witch":
                selectedEnemies.AddRange(stage.witch);
                if (selectedEnemies.Count > 0)
                {
                    bossEnemy = selectedEnemies[selectedEnemies.Count - 1];
                    selectedEnemies.RemoveAt(selectedEnemies.Count - 1);
                }
                break;
        }

        enemyPrefabs = selectedEnemies;

        CustomLogger.Log("선택된 적 프리팹 수: " + enemyPrefabs.Count, "blue");
        CustomLogger.Log("보스 몬스터: " + (bossEnemy != null ? bossEnemy.name : "없음"), "red");
    }
    
}