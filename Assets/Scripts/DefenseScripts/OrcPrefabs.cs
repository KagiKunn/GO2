using UnityEngine;
using System.Collections.Generic;

public class OrcPrefabs : MonoBehaviour
{
    [SerializeField] private List<GameObject> orcPrefabs;
    [SerializeField] private GameObject orcBossPrefab;

    public List<GameObject> GetPrefabs()
    {
        return orcPrefabs;
    }
    
    public GameObject GetBossPrefab()
    {
        return orcBossPrefab;
    }
}