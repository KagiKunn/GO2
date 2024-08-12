using UnityEngine;
using System.Collections.Generic;

public class EnemyPrefabList : MonoBehaviour
{
    // Dark Elf 종족의 적 프리팹 리스트
    [SerializeField] private List<GameObject> darkElfPrefabs = new List<GameObject>();

    // Human 종족의 적 프리팹 리스트
    [SerializeField] private List<GameObject> humanPrefabs = new List<GameObject>();

    // Witch 종족의 적 프리팹 리스트
    [SerializeField] private List<GameObject> witchPrefabs = new List<GameObject>();

    // Orc 종족의 적 프리팹 리스트
    [SerializeField] private List<GameObject> orcPrefabs = new List<GameObject>();

    // Skeleton 종족의 적 프리팹 리스트
    [SerializeField] private List<GameObject> skeletonPrefabs = new List<GameObject>();

    // 선택된 종족에 대한 적 프리팹 배열을 반환하는 메서드
    public GameObject[] GetEnemyPrefabs(string race)
    {
        switch (race)
        {
            case "DarkElf":
                return darkElfPrefabs.ToArray();
            case "Human":
                return humanPrefabs.ToArray();
            case "Witch":
                return witchPrefabs.ToArray();
            case "Orc":
                return orcPrefabs.ToArray();
            case "Skeleton":
                return skeletonPrefabs.ToArray();
            default:
                Debug.LogWarning("해당하는 종족의 프리팹 리스트가 없습니다: " + race);
                return new GameObject[0];
        }
    }
}