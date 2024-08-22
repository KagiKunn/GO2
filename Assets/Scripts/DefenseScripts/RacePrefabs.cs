using UnityEngine;

public abstract class RacePrefabs : ScriptableObject {
	public GameObject[] unitPrefabs; // 일반 적 및 보스 몬스터를 포함한 프리팹 배열

	public GameObject bossPrefab;

	public virtual void Races() {
		CustomLogger.Log("hello");
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