using UnityEngine;
using System.Collections.Generic;

public class SkeletonPrefabs : MonoBehaviour
{
    [SerializeField] private List<GameObject> skeletonPrefabs;
    [SerializeField] private GameObject skeletonBossPrefab;

    public List<GameObject> GetPrefabs()
    {
        return skeletonPrefabs;
    }

    public GameObject GetBossPrefab()
    {
        return skeletonBossPrefab;
    }
}