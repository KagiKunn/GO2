using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "Unit Data", order = 60 )]
public class UnitData : ScriptableObject
{
    public int unitNumber; // 유닛 고유 번호
    public string CharacterClass; // 유닛 클래스(직업)
    public int Attack; // 공격력
    public float AttackSpeed; // 공속
    public float AttackRange; // 공격 사거리
    public GameObject UnitPrefab; // 유닛 프리팹
}
