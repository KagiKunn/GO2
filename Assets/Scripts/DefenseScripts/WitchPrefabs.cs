using UnityEngine;
using System.Collections.Generic;

public class WitchPrefabs : MonoBehaviour
{
    [SerializeField] private List<GameObject> witchPrefabs;
    [SerializeField] private GameObject witchBossPrefab;

    public List<GameObject> GetPrefabs()
    {
        return witchPrefabs;
    }

    public GameObject GetBossPrefab()
    {
        return witchBossPrefab;
    }
}