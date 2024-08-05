using UnityEngine;
using System.Collections.Generic;

public class DarkElfPrefabs : MonoBehaviour
{
    [SerializeField] private List<GameObject> darkElfPrefabs;
    [SerializeField] private GameObject darkElfBossPrefab;

    public List<GameObject> GetPrefabs()
    {
        return darkElfPrefabs;
    }

    public GameObject GetBossPrefab()
    {
        return darkElfBossPrefab;
    }
}