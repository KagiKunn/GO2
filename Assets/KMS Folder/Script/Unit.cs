using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "Unit Data", order = 60)]
public class UnitData : ScriptableObject {
	public int ID; // 유닛 고유 번호
	public int Attack; // 공격력
	public float AttackSpeed; // 공속
	public float AttackRange; // 공격 사거리
	public GameObject UnitPrefab; // 유닛 프리팹
	public Sprite UnitImage; // 유닛의 이미지 (유닛 목록 및 드롭 시 사용)
}