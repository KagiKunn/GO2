using UnityEngine;
using System.Collections.Generic;

public class HumanPrefabs : MonoBehaviour
{
    [SerializeField] private List<GameObject> humanPrefabs;
    [SerializeField] private GameObject humanBossPrefab;

    public List<GameObject> GetPrefabs()
    {
        return humanPrefabs;
    }
    
    public GameObject GetBossPrefab()
    {
        return humanBossPrefab;
    }
}