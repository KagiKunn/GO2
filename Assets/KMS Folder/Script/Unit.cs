using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "Unit Data", order = 60)]
public class UnitData : ScriptableObject {
	public string Name; // 이름
	public GameObject UnitPrefab; // 유닛 프리팹
}