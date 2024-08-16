using UnityEngine;

[CreateAssetMenu(fileName = "NewHero", menuName = "Hero Data", order = 51)]
public class HeroData : ScriptableObject {
	// 영웅 데이터 관리하기 위해 만든 클래스
	public string Name; // 이름
	
	[Header("# Deffence Control")]
	public int HP; // 체력
	public int Attack; // 공격력
	public float AttackSpeed; // 공속
	public Sprite ProfileImg; // 영웅 편성 시 클릭 할 이미지
	public Sprite CharacterImg; // 영웅 편성 시 화면에 나올 전신 이미지
	
	[Header("# Offence Control")]
	public string OffenceHeroAdvantage;
	public int OffenceHP; // 체력
	public int OffenceAttack; // 공격력
	public float OffenceAttackSpeed; // 공속

	[Header("# Color")]
	public float r;
	public float g;
	public float b;
}