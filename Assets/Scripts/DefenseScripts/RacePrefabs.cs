using UnityEngine;

public abstract class RacePrefabs : ScriptableObject
{
    public GameObject[] unitPrefabs;

    public virtual void Races()
    {
        Debug.Log("hello");
    }
}

[CreateAssetMenu(menuName = "RacePrefabs/DarkElfPrefabs")]
public class DarkElfPrefabs : RacePrefabs { }

[CreateAssetMenu(menuName = "RacePrefabs/HumanPrefabs")]
public class HumanPrefabs : RacePrefabs { }

[CreateAssetMenu(menuName = "RacePrefabs/OrcPrefabs")]
public class OrcPrefabs : RacePrefabs { }

[CreateAssetMenu(menuName = "RacePrefabs/SkeletonPrefabs")]
public class SkeletonPrefabs : RacePrefabs { }

[CreateAssetMenu(menuName = "RacePrefabs/WitchPrefabs")]
public class WitchPrefabs : RacePrefabs { }